using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.User
{
    /// <summary>
    /// 信息对象
    /// 创建用户
    /// </summary>
    public class CreateUser
    {
        public string userName { set; get; }
        public string userDepartment { set; get; }
        public string userJob { set; get; }
        public string userDescription { set; get; }
        public int roleID { set; get; }
    }
}