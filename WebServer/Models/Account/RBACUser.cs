using System;
using System.Collections.Generic;
using System.Linq;
using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;

namespace WebServer.Models
{
    public class RolePermission
    {
        public int permissionID { set; get; }
        public string PermissionDescription { set; get; }
    }
    public class UserRole
    {
        public int roleID { set; get; }
        public string roleName { set; get; }
        public List<RolePermission> permissions = new List<RolePermission>();
    }
    public class RBACUser
    {
        public int userID { set; get; }
        public string userName { set; get; }
        private List<UserRole> roles = new List<UserRole>();

        public RBACUser(string _userName)
        {
            this.userName = _userName;
            GetDatabaseUserRolesPermissions();
        }

        private void GetDatabaseUserRolesPermissions()
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            //根据用户名从数据库中填充角色列表和权限列表
            PersonDAO personDao = Factory.getInstance<PersonDAO>();

            wherelist.Add("personName", this.userName);
            PersonVO personVo = personDao.getOne<PersonVO>(wherelist);
            if (personVo == null) return;
            this.userID = personVo.personID;

            Person_RoleDAO person_roleDao = Factory.getInstance<Person_RoleDAO>();
            wherelist.Clear();
            wherelist.Add("personID", userID);
            //获取用户、角色关联
            List<Person_RoleVO> person_roleVolist = person_roleDao.getAll<Person_RoleVO>(wherelist);
            if (person_roleVolist == null) return;

            foreach (Person_RoleVO person_roleVo in person_roleVolist)
            {
                UserRole userRole = new UserRole();

                userRole.roleID = person_roleVo.roleID;
                RoleDAO roleDAO = Factory.getInstance<RoleDAO>();
                //获取角色信息
                RoleVO roleVo = roleDAO.getOne<RoleVO>(userRole.roleID);
                if (roleVo == null) continue;
                userRole.roleID = roleVo.roleID;
                userRole.roleName = roleVo.roleName;

                Role_PermissionDAO rolePermissionDAO = Factory.getInstance<Role_PermissionDAO>();
                
                wherelist.Clear();
                wherelist.Add("roleID",roleVo.roleID);
                //获取角色、权限关联
                List<Role_PermissionVO> role_permissionVolist = rolePermissionDAO.getAll<Role_PermissionVO>(wherelist);
                if (role_permissionVolist == null) continue;

                foreach (Role_PermissionVO role_permissionVo in role_permissionVolist)
                {
                    RolePermission rolePermission = new RolePermission();

                    rolePermission.permissionID = role_permissionVo.permissionID;
                    PermissionDAO permissionDAO = Factory.getInstance<PermissionDAO>();
                    //获取权限信息
                    PermissionVO permissionVo = permissionDAO.getOne<PermissionVO>(role_permissionVo.permissionID);
                    if (permissionVo == null) continue;

                    rolePermission.PermissionDescription = permissionVo.permissionDescription;

                    userRole.permissions.Add(rolePermission);
                }
                roles.Add(userRole);
            }
        }

        public bool HasPermission(string requirePermission)
        {
            bool bFound = false;
            foreach (UserRole role in this.roles)
            {
                bFound = (role.permissions.Where(
                    p => p.PermissionDescription == requirePermission).ToList().Count > 0);
                if (bFound)
                {
                    break;
                }
            }
            return bFound;
        }

        public bool HasRole(string role)
        {
            return (roles.Where(p => p.roleName == role).ToList().Count > 0);
        }

        public bool HasRoles(string roles)
        {
            bool bFound = false;
            string[] _roles = roles.ToLower().Split(';');
            foreach (UserRole role in this.roles)
            {
                try
                {
                    bFound = _roles.Contains(role.roleName.ToLower());
                    if (bFound)
                    {
                        return bFound;
                    }
                }
                catch (Exception)
                {

                }
            }
            return bFound;
        }
    }
}