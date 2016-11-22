using System.Collections.Generic;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.User;

namespace WebServer.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        [HttpGet]
        public ActionResult Index_admin()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add_admin()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit_admin()
        {
            return View();
        }

        public JsonResult GetUsersForDelegate()
        {
            RespondModel respond = new RespondModel();
            //调用会场服务
            List<UserForDelegate> list = null;
            Status status = UserService.getAllForDelegate(out list);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = list;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
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

        [HttpGet]
        public JsonResult GetUserForUpdate(int userID)
        {
            RespondModel respond = new RespondModel();

            UpdateUser user;
            //调用用户服务
            Status status = UserService.getOneUpdate(out user, userID);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = user;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateUser(CreateUser user)
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            Status status = new UserService().create(user);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
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

        [HttpPost]
        public JsonResult UpdateUserAvailable(int userID, int state)//state对应数据库中的available字段
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            Status status = UserService.UpdateUserAvailable(userID, state);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
