using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using DAL.DAOVO;
using WebServer.Models.User;

namespace WebServer.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetUsers()
        {
            RespondModel respond = new RespondModel();

            Users users;
            //调用用户服务
            respond.Code = UserService.getAll(out users);
            respond.Result = users;

            if (respond.Code == 1)
            {
                respond.Message = "success";
            }
            else
            {
                respond.Message = "failed";
            }

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Createuser(HttpContext context)
        {
            RespondModel respond = new RespondModel();
            //调用json解析
            User user = Models.Tools.JsonHelper.GetObjectService<User>(context);
            //调用用户服务
            respond.Code = UserService.create(user);

            if (respond.Code == 1)
            {
                respond.Message = "success";
            }
            else
            {
                respond.Message = "failed";
            }

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateUsers(HttpContext context)
        {
            RespondModel respond = new RespondModel();
            //调用json解析
            Users users = Models.Tools.JsonHelper.GetObjectService<Users>(context);
            //调用用户服务
            respond.Code = UserService.update(users);
            if (respond.Code == 1)
            {
                respond.Message = "success";
            }
            else
            {
                respond.Message = "failed";
            }


            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUsers(HttpContext context)
        {
            RespondModel respond = new RespondModel();
            //调用json解析
            OldUsers users = Models.Tools.JsonHelper.GetObjectService<OldUsers>(context);
            //调用用户服务
            respond.Code = UserService.delete(users);

            if (respond.Code == 1)
            {
                respond.Message = "success";
            }
            else
            {
                respond.Message = "failed";
            }
            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
