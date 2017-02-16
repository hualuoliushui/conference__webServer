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
using DAL.DAO;
using WebServer.Models.MeetingPlace;
using WebServer.Models.Device;
using WebServer.Models.User;
using WebServer.Models.Role;

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
            int meetingPlaceNum, deviceNum, userNum, roleNum;

            List<MeetingPlace> meetingPlaces = null;
            //调用会场服务
            new MeetingPlaceService().getAll(out meetingPlaces);
            meetingPlaceNum = meetingPlaces.Count;

            List<Device> devices;
            //调用设备服务
            new DeviceService().getAll(out devices);
            deviceNum = devices.Count;

            List<User> users;
            //调用用户服务
            new UserService().getAll(out users);
            userNum = users.Count;

            //调用角色服务
            List<RoleForUser> roles = null;
            new RoleService().getAllForUser(out roles);
            roleNum = roles.Count;
            return View(Tuple.Create<int,int,int,int>(meetingPlaceNum,deviceNum,userNum,roleNum));
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
