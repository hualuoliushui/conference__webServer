using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAO;
using DAL.DAOFactory;
using DAL.DAOVO;

namespace WebServer.Models.Role
{
    public class RoleService
    {
        /// <summary>
        /// 角色名
        /// 长度规范
        /// </summary>
        private static int RoleNameMin = 2;
        private static int RoleNameMax = 12;

        /// <summary>
        /// 检查角色长度规范
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        private bool checkFormat(string roleName)
        {
            return roleName.Length >= 2 && roleName.Length <= 12;
        }

        /// <summary>
        /// 为用户管理返回角色列表
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public Status getAllForUser(out List<RoleForUser> roles)
        {
            roles = new List<RoleForUser>();

            RoleDAO roleDao = Factory.getInstance<RoleDAO>();
            List<RoleVO> roleVolist = roleDao.getAll<RoleVO>();
            if (roleVolist == null )
                return Status.NONFOUND;
            foreach (RoleVO vo in roleVolist)
            {
                roles.Add(
                    new RoleForUser { 
                        roleID = vo.roleID, 
                        roleName = vo.roleName 
                    });
            }
            return Status.SUCCESS;
        }

        /// <summary>
        /// 为创建角色返回权限列表
        /// </summary>
        /// <param name="hasPermission"></param>
        /// <returns></returns>
        public Status getPermissions(out List<Permission> permissions)
        {
            permissions = new List<Permission>();

            PermissionDAO permissionDao = Factory.getInstance<PermissionDAO>();
            List<PermissionVO> permissionVos = permissionDao.getAll<PermissionVO>();
            if (permissionVos.Count == 0)
                return Status.NONFOUND;


            foreach (PermissionVO vo in permissionVos)
            {
                permissions.Add(
                    new Permission
                    {
                        permissionID = vo.permissionID,
                        permissionName = vo.permissionName
                    });
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 展示所有角色及其权限
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public Status getAll(out Roles roles)
        {
            roles = new Roles();
            roles.roles = new List<Role>();

            //获取所有的权限列表：permissionID,permissionName
            List<Permission> permissionlist;
            if ( new RoleService().getPermissions(out permissionlist) != Status.SUCCESS)
            {
                return Status.NONFOUND;
            }

            roles.permissonNum = permissionlist.Count;

            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            RoleDAO roleDao = Factory.getInstance<RoleDAO>();
            PermissionDAO permissionDao = Factory.getInstance<PermissionDAO>();
            Role_PermissionDAO role_PermissionDao = Factory.getInstance<Role_PermissionDAO>();

            //获取所有的角色列表:roleID,roleName
            List<RoleVO> roleVolist = roleDao.getAll<RoleVO>();
            if (roleVolist == null)
                return Status.NONFOUND;

            foreach (RoleVO roleVo in roleVolist)
            {
                if (string.Compare(roleVo.roleName, "管理员") == 0
                    || string.Compare(roleVo.roleName, "管理员") == 0)
                {
                    continue;
                }
                //初始化当前角色权限列表
                int[] hasPermissionlist = new int[permissionlist.Count];
                //获取角色、权限关联
                wherelist.Clear();
                wherelist.Add("roleID", roleVo.roleID);
                List<Role_PermissionVO> role_permissionVolist = role_PermissionDao.getAll<Role_PermissionVO>(wherelist);

                List<Permission> permissions = new List<Permission>();
                if (role_permissionVolist != null)
                {
                    foreach (Role_PermissionVO vo in role_permissionVolist)
                    {
                        for (int i = 0; i < permissionlist.Count; i++)
                        {
                            if (permissionlist[i].permissionID == vo.permissionID)
                            {
                                hasPermissionlist[i] = 1;
                            }
                        }
                    }
                }
                roles.roles.Add(
                        new Role
                        {
                            roleID = roleVo.roleID,
                            roleName = roleVo.roleName,
                            hasPermission = hasPermissionlist
                        });    
            }

            return Status.SUCCESS;
        }


        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public Status create(CreateRole role)
        {
            if (string.IsNullOrWhiteSpace(role.roleName))
            {
                return Status.ARGUMENT_ERROR;
            }
            //修正字符串
            role.roleName = role.roleName.Trim();
            //检查长度规范
            if (!checkFormat(role.roleName))
            {
                return Status.FORMAT_ERROR;
            }

            RoleDAO roleDao = Factory.getInstance<RoleDAO>();
            Role_PermissionDAO role_PermissionDao = Factory.getInstance<Role_PermissionDAO>();

            //不允许添加无权限角色
            if (role.permissionIDs == null || role.permissionIDs.Count ==0 )
            {
                return Status.ARGUMENT_ERROR;
            }

            //插入角色
            int roleID = RoleDAO.getID();
            if (roleDao.insert<RoleVO>(
                new RoleVO {
                    roleID = roleID, 
                    roleName = role.roleName 
                }) != 1)
            {
                return Status.FAILURE;
            }
            //插入角色、权限关联
            Queue<int> role_permissionIDs = new Queue<int>();
            for (int i = 0; i < role.permissionIDs.Count; i++)
            {
                int id = Role_PermissionDAO.getID();
                if (role_PermissionDao.insert<Role_PermissionVO>(
                                    new Role_PermissionVO
                                    {
                                        role_permissionID = id,
                                        roleID = roleID,
                                        permissionID = role.permissionIDs[i]
                                    }) != 1)
                { //如果失败，，回退
                    while(role_permissionIDs.Count != 0)
                    {
                        id = role_permissionIDs.Dequeue();
                        role_PermissionDao.delete(id);
                    }
                    roleDao.delete(roleID);

                    return Status.FAILURE;
                }
                role_permissionIDs.Enqueue(id);
            }
                
            return Status.SUCCESS;
        }

        public Status delete(List<int> roleIDs){
            if (roleIDs.Count == 0)
            {
                return Status.SUCCESS;
            }

            RoleDAO roleDao = Factory.getInstance<RoleDAO>();

            Role_PermissionDAO role_PermissionDao = Factory.getInstance<Role_PermissionDAO>();
            Dictionary<string,object> wherelist =new Dictionary<string,object>();
            foreach (int roleID in roleIDs)
            {
                //获取角色信息
                RoleVO roleVo = roleDao.getOne<RoleVO>(roleID);
                if (string.Compare(roleVo.roleName, "管理员") == 0//禁止删除管理员角色
                    || string.Compare(roleVo.roleName, "会议组织者") == 0//禁止删除组织者角色
                    || string.Compare(roleVo.roleName, "成员") == 0)//禁止删除成员角色
                    return Status.PERMISSION_DENIED;
                wherelist.Clear();
                wherelist.Add("roleID",roleID);
                role_PermissionDao.delete(wherelist);
            }

            return Status.SUCCESS;
        }
    }
}