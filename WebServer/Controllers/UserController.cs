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

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public JsonResult GetUsers()
        {
            RespondModel respond = new RespondModel();

            List<User> users;
            //调用用户服务
            Status status = UserService.getAll(out users);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = users;

           return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUser(int userID)
        {
            RespondModel respond = new RespondModel();

            UpdateUser user;
            //调用设备服务
            Status status = UserService.getOne(out user, userID);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = user;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateUser(CreateUser user)
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            Status status = UserService.create(user);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateUser(UpdateUser user)
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            Status status = UserService.update(user);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUsers(List<int> users)
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            Status status = UserService.delete(users);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
