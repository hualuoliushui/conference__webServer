using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.Meeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.App_Start;

namespace WebServer.Controllers
{
    public class MeetingController : Controller
    {
        //
        // GET: /Meeting/

        public ActionResult Index()
        {
            return View();
        }
        [RBAC]
        public JsonResult GetMeetings()
        {
            RespondModel respond = new RespondModel();

            List<TipMeeting> meetings = null;
            //调用服务
            Status status = new MeetingService().getAll(out meetings);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = meetings;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
        [RBAC]
        public JsonResult GetMeeting(int meetingID)
        {
            RespondModel respond = new RespondModel();

            Meeting meeting = null;
            //调用服务
            Status status = new MeetingService().getOne(meetingID, out meeting);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = meeting;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
        [RBAC]
        public JsonResult CreateMeeting(CreateMeeting meeting)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new MeetingService().create(userName, meeting);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
        [RBAC]
        public JsonResult GetMeetingForUpdate(int meetingID)
        {
            RespondModel respond = new RespondModel();

            UpdateMeeting meeting = null;
            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new MeetingService().getOneForUpdate(userName, meetingID, out meeting);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = meeting;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
        [RBAC]
        public JsonResult UpdateMeeting(UpdateMeeting meeting)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new MeetingService().update(userName, meeting);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [RBAC]
        public JsonResult DeleteMeetingMultipe(List<int> meetingIDs)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new MeetingService().deleteMultipe(userName, meetingIDs);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
