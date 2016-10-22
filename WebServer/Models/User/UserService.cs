using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOVO;

namespace WebServer.Models.User
{
    public class UserService
    {
        public static int create(User user)
        {
            return 1;
        }

        public static int getAll(out Users users)
        {
            users = new Users();
            users.users = new List<User>();

            return 1;
        }

        public static int update(Users users)
        {

            return 1;
        }

        public static int delete(OldUsers users)
        {

            return 1;
        }
    }
}