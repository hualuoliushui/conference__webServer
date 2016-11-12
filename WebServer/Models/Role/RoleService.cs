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
        private static bool checkFormat(string roleName)
        {
            return roleName.Length >= 2 && roleName.Length <= 12;
        }

        private static int RoleIDMax = Factory.getRoleDAOInstance().getIDMax();

        /// <summary>
        /// 获取角色ID
        /// </summary>
        /// <returns></returns>
        private static int getRoleID()
        {
            int roleID = 0;
            Object lockObject = new object();
            lock (lockObject)
            {
                roleID = ++RoleIDMax;
            }
            return roleID;
        }

        /// <summary>
        /// 为用户管理返回角色列表
        /// </summary>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static Status getAllForUser(out List<RoleForUser> roles)
        {
            roles = new List<RoleForUser>();

            RoleDAO roleDaoProxy = Factory.getRoleDAOInstance();
            List<RoleVO> roleVos = roleDaoProxy.getRoleList();
            if (roleVos.Count==0)
                return Status.NONFOUND;
            foreach (RoleVO vo in roleVos)
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
        /// <param name="permissions"></param>
        /// <returns></returns>
        public static Status getPermissions(out List<Permission> permissions)
        {
            permissions = new List<Permission>();

            PermissionDAO permissionDao = Factory.getPermissionDAOInstance();
            List<PermissionVO> permissionVos = permissionDao.getPermissionList();
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
        public static Status getAll(out List<Role> roles)
        {
            roles = new List<Role>();

            RoleDAO roleDao = Factory.getRoleDAOInstance();
            PermissionDAO permissionDao = Factory.getPermissionDAOInstance();
            Role_PermissionDAO role_PermissionDao = Factory.getRole_PermissionDAOInstance();

            List<RoleVO> roleVos = roleDao.getRoleList();
            if (roleVos.Count == 0)
                return Status.NONFOUND;
            foreach (RoleVO roleVo in roleVos)
            {
                List<int> permissionIDs = role_PermissionDao.getPermissionIDListByRoleID(roleVo.roleID);
                if (permissionIDs.Count == 0)
                    continue;
                List<Permission> permissions = new List<Permission>();
                foreach (int permissionID in permissionIDs)
                {
                    PermissionVO vo = permissionDao.getPermissionByPermissionID(permissionID);
                    if (vo == null)
                        continue;
                    permissions.Add(
                        new Permission
                        {
                            permissionID = permissionID,
                            permissionName = vo.permissionName
                        });
                }

                roles.Add(
                    new Role
                    {
                        roleID = roleVo.roleID,
                        roleName = roleVo.roleName,
                        permissions = permissions
                    });
            }

            return Status.SUCCESS;
        }


        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static Status create(CreateRole role)
        {
            //修正字符串
            role.roleName = role.roleName.Trim();
            //检查长度规范
            if (!checkFormat(role.roleName))
            {
                return Status.FORMAT_ERROR;
            }

            RoleDAO roleDao = Factory.getRoleDAOInstance();
            Role_PermissionDAO role_PermissionDao = Factory.getRole_PermissionDAOInstance();

            if (role.permissionIDs == null || role.permissionIDs.Count == 0)
            {
                return Status.ARGUMENT_ERROR;
            }

            int roleID = getRoleID();
            if (!roleDao.addRole(new RoleVO { roleID = roleID, roleName = role.roleName }))
            {
                return Status.FAILURE;
            }

            for (int i = 0; i < role.permissionIDs.Count; i++)
            {
                if (!role_PermissionDao.addRole_Permission(
                                    new Role_PermissionVO
                                    {
                                        roleID = roleID,
                                        permissionID = role.permissionIDs[i]
                                    }))
                {
                    for (int j = --i; j >= 0; j--)
                    {
                        role_PermissionDao.
                            deleteRole_PermissionByRoleIDAndPermissionID(
                            roleID, role.permissionIDs[j]);
                    }
                    roleDao.deleteRoleByRoleID(roleID);

                    return Status.FAILURE;
                }
            }
                
            return Status.SUCCESS;
        }

        public static Status delete(List<int> roleIDs){
            if (roleIDs.Count == 0)
            {
                return Status.SUCCESS;
            }

            RoleDAO roleDao = Factory.getRoleDAOInstance();
            Role_PermissionDAO role_PermissionDao = Factory.getRole_PermissionDAOInstance();

            foreach (int roleID in roleIDs)
            {
                role_PermissionDao.deleteRole_PermissionByRoleID(roleID);
            }

            return Status.SUCCESS;
        }
    }
}