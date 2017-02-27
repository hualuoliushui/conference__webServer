using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.User
{
    /// <summary>
    /// 信息对象
    /// 获取用户信息
    /// </summary>
    public class User
    {
        public int userID { set; get; }
        public string userName { set; get; }
        public string userDepartment { set; get; }
        public string userJob { set; get; }
        public string roleName { set; get; }
        public int userFreezeState { set; get; }
        public int userLevel { set; get; }
    }
}