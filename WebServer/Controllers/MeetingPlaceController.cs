using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.MeetingPlace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Controllers
{
    public class MeetingPlaceController : Controller
    {
        //
        // GET: /MeetingPlace/

        public ActionResult Index()
        {
            return View();
        }

        //返回会场列表
        public JsonResult GetMeetingPlaces()
        {
            RespondModel respond = new RespondModel();

            List<MeetingPlace> meetingPlaces = null;
            //调用会场服务
            Status status = MeetingPlaceService.getAll(out meetingPlaces);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = meetingPlaces;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMeetingPlace(int meetingPlaceID)
        {
            RespondModel respond = new RespondModel();

            UpdateMeetingPlace meetingPlace;
            //调用设备服务
            Status status = MeetingPlaceService.getOne(out meetingPlace, meetingPlaceID);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = meetingPlace;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        //添加会场信息
        public JsonResult CreateMeetingPlace(CreateMeetingPlace meetingPlace)
        {
            RespondModel respond = new RespondModel();
            //调用会场服务
            Status status = new MeetingPlaceService().create(meetingPlace);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        //更新会场信息
        public JsonResult UpdateMeetingPlaces(UpdateMeetingPlace meetingPlace)
        {
            RespondModel respond = new RespondModel();

            Status status = MeetingPlaceService.update(meetingPlace);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateMeetingPlaceAvailable(int meetingPlaceID, int meetingPlaceFreezeState) //FreezeState对应数据库中的available字段
        {
            RespondModel respond = new RespondModel();

            Status status = MeetingPlaceService.UpdateUserAvailable(meetingPlaceID, meetingPlaceFreezeState);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
