using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.MeetingPlace;
using System.Collections.Generic;
using WebServer.App_Start;
using WebServer.Models.Tools;

namespace WebServer.Controllers
{
    public class MeetingPlaceController : Controller
    {

        public ActionResult Index_admin()
        {
            return View();
        }

        public ActionResult Add_admin()
        {
            return View();
        }

        public ActionResult Edit_admin()
        {
            return View();
        }
        [RBAC]
        public JsonResult GetMeetingPlacesForMeeting()
        {
            RespondModel respond = new RespondModel();
            //调用会场服务
            List<MeetingPlaceForMeeting> list = null;
            Status status = new MeetingPlaceService().getAllForMeeting(out list);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = list;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        //返回会场列表
        [RBAC]
        public JsonResult GetMeetingPlaces()
        {
            RespondModel respond = new RespondModel();

            List<MeetingPlace> meetingPlaces = null;
            //调用会场服务
            Status status = new MeetingPlaceService().getAll(out meetingPlaces);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = meetingPlaces;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
        [RBAC]
        public JsonResult GetMeetingPlaceForUpdate(int meetingPlaceID)
        {
            RespondModel respond = new RespondModel();

            UpdateMeetingPlace meetingPlace;
            //调用设备服务
            Status status = new MeetingPlaceService().getOneForUpdate(out meetingPlace, meetingPlaceID);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = meetingPlace;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        //添加会场信息
        [RBAC]
        public JsonResult CreateMeetingPlace(CreateMeetingPlace meetingPlace)
        {
            if (ModelState.IsValid)
            {
                //调用会场服务
                Status status = new MeetingPlaceService().create(meetingPlace);
                return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
            }

            return Json(new RespondModel(Status.ARGUMENT_ERROR, ModelStateHelper.errorMessages(ModelState)), JsonRequestBehavior.AllowGet);
        }

        //更新会场信息
        [RBAC]
        public JsonResult UpdateMeetingPlace(UpdateMeetingPlace meetingPlace)
        {
            if (ModelState.IsValid)
            {
                //调用会场服务
                Status status = new MeetingPlaceService().update(meetingPlace);
                return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
            }

            return Json(new RespondModel(Status.ARGUMENT_ERROR, ModelStateHelper.errorMessages(ModelState)), JsonRequestBehavior.AllowGet);
        }

        [RBAC]
        public JsonResult UpdateMeetingPlaceAvailable(int meetingPlaceID, int state) //state对应数据库中的available字段
        {
            RespondModel respond = new RespondModel();

            Status status = new MeetingPlaceService().UpdateUserAvailable(meetingPlaceID, state);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
