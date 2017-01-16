using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.App_Start;
using WebServer.Models.Meeting;
using WebServer.Models.Account;
using WebServer.Models.Tools;

namespace WebServer.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (!Forms.Verigy(model.userName, model.password))
                {
                    RespondModel respond = new RespondModel();
                    respond.Code = -1;
                    respond.Message = "用户名或密码出错，请重新输入";
                    return Json(respond, JsonRequestBehavior.AllowGet);
                }

                Session["userName"] = model.userName;

                return Json(new RespondModel(Status.SUCCESS, ""), JsonRequestBehavior.AllowGet);
            }

            return Json(new RespondModel(Status.ARGUMENT_ERROR, ModelStateHelper.errorMessages(ModelState)), JsonRequestBehavior.AllowGet);

        }

        [RBAC]
        [HttpGet]
        public ActionResult Admin()
        {
            return View("Admin");
        }

        [RBAC]
        [HttpGet]
        public ActionResult Organizor()
        {
            List<TipMeeting> meetings = null;
            //调用服务
            Status status = new MeetingService().getAll(out meetings);

            return View(meetings);
        }

        public JsonResult Logout()
        {
            Status status = Status.SUCCESS;

            if (Session["userName"] != null)
            {
                Forms.Logout((string)Session["userName"]);
                Session.Clear();
            }

            return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
        }
    }
}
