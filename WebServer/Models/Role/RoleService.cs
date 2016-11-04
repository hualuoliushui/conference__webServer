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
        public static Status getAllForUser(out List<RoleForUser> roles)
        {
            roles = new List<RoleForUser>();

            RoleDAO roleDaoProxy = Factory.getRoleDAOInstance();
            List<RoleVO> roleVos = roleDaoProxy.getRoleList();
            if (roleVos == null)
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
    }
}