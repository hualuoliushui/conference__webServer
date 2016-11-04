using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.MeetingPlace;

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

            MeetingPlaces meetingPlaces;
            //调用会场服务
            respond.Code = MeetingPlaceService.getAll(out meetingPlaces);
            respond.Result = meetingPlaces;

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

        public JsonResult GetMeetingPlace(int meetingPlaceID)
        {
            RespondModel respond = new RespondModel();

            MeetingPlace meetingPlace;
            //调用设备服务
            respond.Code = MeetingPlaceService.getOne(out meetingPlace, meetingPlaceID);
            respond.Result = meetingPlace;

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

        //添加会场信息
        public JsonResult CreateMeetingPlace(HttpContext context)
        {
            RespondModel respond = new RespondModel();
            //调用json解析
            NewMeetingPlace meetingPlace = Models.Tools.JsonHelper.GetObjectService<NewMeetingPlace>(context);
            //调用会场服务
            respond.Code = MeetingPlaceService.create(meetingPlace);

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

        //更新会场信息
        public JsonResult UpdateMeetingPlaces(HttpContext context)
        {
            RespondModel respond = new RespondModel();
            //调用json解析
            MeetingPlaces meetingPlaces = Models.Tools.JsonHelper.GetObjectService<MeetingPlaces>(context);
            //调用会场服务
            respond.Code = MeetingPlaceService.update(meetingPlaces);
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

        //删除会场信息
        public JsonResult DeleteMeetingPlaces(HttpContext context)
        {
            RespondModel respond = new RespondModel();
            //调用json解析
            OldMeetingPlaces oldMeetingPlaces = Models.Tools.JsonHelper.GetObjectService<OldMeetingPlaces>(context);
            //调用会场服务
            respond.Code = MeetingPlaceService.delete(oldMeetingPlaces);
            // respond.Result = oldMeetingPlaces; //测试接口时使用
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
