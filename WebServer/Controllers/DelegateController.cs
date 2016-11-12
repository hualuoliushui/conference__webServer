using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.Delegate;
using DAL.DAOVO;

namespace WebServer.Controllers
{
    public class DelegateController : Controller
    {
        //
        // GET: /Delegate/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDelegates(int meetingID)
        {
            RespondModel respond = new RespondModel();

            List<Models.Delegate.Delegate> delegates;
            //调用设备服务
            Status status = DelegateService.getAll(meetingID,out delegates);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = delegates;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateDelegateMultipe(List<CreateDelegate> delegates)
        {
            RespondModel respond = new RespondModel();

            //调用会场服务
            string userName = HttpContext.User.Identity.Name;
            Status status = DelegateService.createMultiple(userName, delegates);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateDelegate(CreateDelegate createDelegate)
        {
            RespondModel respond = new RespondModel();

            //调用会场服务
            string userName = HttpContext.User.Identity.Name;
            Status status = DelegateService.create(userName, createDelegate);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDelegate(UpdateDelegate updateDelegate)
        {
            RespondModel respond = new RespondModel();

            //调用会场服务
            string userName = HttpContext.User.Identity.Name;
            Status status = DelegateService.update(userName, updateDelegate);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDelegateMultipe(List<DeleteDelegate> delegates)
        {
            RespondModel respond = new RespondModel();

            //调用会场服务
            string userName = HttpContext.User.Identity.Name;
            Status status = DelegateService.deleteMultipe(userName, delegates);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
