using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Role
{
    public class CreateRole
    {
        public string roleName { set; get; }
        public List<int> permissionIDs { set; get; }
    }
}