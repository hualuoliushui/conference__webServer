using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.User
{
    public class CreateUserForDelegate
    {
        public string userName { set; get; }
        public string userDepartment { set; get; }
        public string userJob { set; get; }
        public string userDescription { set; get; }
    }
}