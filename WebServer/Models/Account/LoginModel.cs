using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.Account
{
    public class LoginModel
    {
        [Required(ErrorMessage="用户名不为空")]
        public string userName { set; get; }

        [Required(ErrorMessage="密码不为空")]
        [DataType(DataType.Password)]
        public string password { set; get; }
    }
}