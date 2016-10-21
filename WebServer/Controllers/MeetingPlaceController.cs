using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public JsonResult GetMeetingPlaces()
        {
            RespondModel respond = new RespondModel();

            MeetingPlaces meetingPlaces;
            respond.Code = MeetingPlaces.getMeetingPlaces(out meetingPlaces);           

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

        public JsonResult CreateMeetingPlace()
        {
            RespondModel respond = new RespondModel();

            using (var reader = new System.IO.StreamReader(Request.InputStream))
            {
                String jsonData = reader.ReadToEnd();
                //jsonData = "{\"meetingPlaceName\": \"人民大会堂\",\"meetingPlaceType\": \"主席台型\",\"meetingPlaceCapacity\": 200}";
                if (!string.IsNullOrEmpty(jsonData))
                {   
                    //Json字符串解析
                    NewMeetingPlace newMeetingPlace = JsonConvert.DeserializeObject<NewMeetingPlace>(jsonData);
                    //数据库执行函数
                    respond.Code =  NewMeetingPlace.createMeetingPlace(newMeetingPlace);
                    //respond.Result = newMeetingPlace; //测试接口时使用
                    if (respond.Code == 1)
                    {
                        respond.Message = "success";
                    }
                    else
                    {
                        respond.Message = "failed";
                    }
                }
            }
            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateMeetingPlaces()
        {
            RespondModel respond = new RespondModel();
            using (var reader = new System.IO.StreamReader(Request.InputStream))
            {
                String jsonData = reader.ReadToEnd();
                //jsonData = "{\"meetingPlaceID\": 1,\"meetingPlaceName\": \"人民大会堂\",\"meetingPlaceType\": \"主席台型\",\"meetingPlaceCapacity\": 200}";
                if (!string.IsNullOrEmpty(jsonData))
                {
                    //Json字符串解析
                    OldMeetingPlaces oldMeetingPlaces = JsonConvert.DeserializeObject<OldMeetingPlaces>(jsonData);
                    //数据库执行函数
                    respond.Code = OldMeetingPlaces.deleteMeetingPlaces(oldMeetingPlaces);
                    // respond.Result = oldMeetingPlaces; //测试接口时使用
                    if (respond.Code == 1)
                    {
                        respond.Message = "success";
                    }
                    else
                    {
                        respond.Message = "failed";
                    }
                }
            }
            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteMeetingPlaces()
        {
            RespondModel respond = new RespondModel();
            using (var reader = new System.IO.StreamReader(Request.InputStream))
            {
                String jsonData = reader.ReadToEnd();
                //jsonData = "{\"meetingPlaces\":[1,2,3,4]}";
                if (!string.IsNullOrEmpty(jsonData))
                {
                    //Json字符串解析
                    OldMeetingPlaces oldMeetingPlaces = JsonConvert.DeserializeObject<OldMeetingPlaces>(jsonData);
                    //数据库执行函数
                    respond.Code = OldMeetingPlaces.deleteMeetingPlaces(oldMeetingPlaces);
                   // respond.Result = oldMeetingPlaces; //测试接口时使用
                    if (respond.Code == 1)
                    {
                        respond.Message = "success";
                    }
                    else
                    {
                        respond.Message = "failed";
                    }
                }
            }
            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
