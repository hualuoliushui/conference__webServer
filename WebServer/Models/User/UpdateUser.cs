using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.User
{
    /// <summary>
    /// 信息对象
    /// 更新用户
    /// </summary>
    public class UpdateUser
    {
        [Required(ErrorMessage = "用户ID不为空")]
        public int userID { set; get; }

        [Required(ErrorMessage = "用户名称不为空")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "用户名称2-100个字符")]
        public string userName { set; get; }

        [Required(ErrorMessage = "用户部门不为空")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "用户部门2-100个字符")]
        public string userDepartment { set; get; }

        [Required(ErrorMessage = "用户职位不为空")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "用户职位2-100个字符")]
        public string userJob { set; get; }

        [StringLength(1000, MinimumLength = 1, ErrorMessage = "用户描述0-1000个字符")]
        public string userDescription { set; get; }

        public int roleID { set; get; }

        public int userLevel { set; get; }
    }
}