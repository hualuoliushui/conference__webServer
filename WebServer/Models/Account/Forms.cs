using System;
using System.Web;
using System.Security.Principal;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using DAL.DAOFactory;
using DAL.DAO;
using DAL.DAOVO;
using System.Collections.Generic;

namespace WebServer.Models
{
    public class Forms
    {
        public int userID;

        private static string MD5(string password)
        {
            byte[] temp = Encoding.Default.GetBytes(password.Trim());
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(temp);
            return Encoding.Default.GetString(output);
        }

        /// <summary>
        /// 执行登录前的验证
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        public static bool Verigy(string loginName, string password)
        {
            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            wherelist.Add("personName", loginName);

            PersonVO person = personDao.getOne<PersonVO>(wherelist);
            if (person == null) 
                return false;
            if (person.personPassword.CompareTo((password)) != 0) 
                return false;

            Login(person.personName, 1440);

            return true;
        }

        public static void Login(string userName, int expiration)
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
                throw new InvalidOperationException();

            RBACUser user = new RBACUser(userName);

            context.Session["user"] = user;

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
               2, userName, DateTime.Now, DateTime.Now.AddDays(1), true, "");

            string cookieValue = FormsAuthentication.Encrypt(ticket);

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                cookieValue);
            cookie.HttpOnly = true;
            cookie.Secure = FormsAuthentication.RequireSSL;
            cookie.Domain = FormsAuthentication.CookieDomain;
            cookie.Path = FormsAuthentication.FormsCookiePath;
            if (expiration > 0)
                cookie.Expires = DateTime.Now.AddMinutes(expiration);

            context.Response.Cookies.Remove(cookie.Name);
            context.Response.Cookies.Add(cookie);
        }

        public static void Logout(string userName)
        {
            HttpContext context = HttpContext.Current;
            if (context == null)
                throw new InvalidOperationException();
            context.Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
        }
    }
}