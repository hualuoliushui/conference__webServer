using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.Role
{
    public class CreateRole
    {
        [Required(ErrorMessage = "角色名称不为空")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "角色名称2-100个字符")]
        public string roleName { set; get; }

        public List<int> permissionIDs { set; get; }
    }
}