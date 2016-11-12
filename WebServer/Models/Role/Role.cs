using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Role
{
    public class Role
    {
        public int roleID { set; get; }
        public string roleName { set; get; }
        public List<Permission> permissions;
    }
}