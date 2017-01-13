using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.Meeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.App_Start;
using WebServer.Models.Agenda;
using WebServer.Models.Delegate;
using WebServer.Models.MeetingPlace;
using WebServer.Models.User;
using WebServer.Models.Device;
using WebServer.Models.Tools;

namespace WebServer.Controllers
{
    public class MeetingController : Controller
    {
        [HttpGet]
        [RBAC]
        public ActionResult Add_organizor()
        {
            MeetingPlaceService meetingPlaceService = new MeetingPlaceService();
            List<MeetingPlaceForMeeting> meetingPlaces = null;
            meetingPlaceService.getAllForMeeting(out meetingPlaces);

            MeetingInfo meeting = new MeetingInfo();
            meeting.meetingToStartTime = DateTime.Now.AddDays(1);
            meeting.meetingStartedTime = DateTime.Now.AddDays(2) ;

            UserService userService = new UserService();
            List<UserForDelegate> users = new List<UserForDelegate>();
            userService.getAllForDelegate(out users);

            ShowMeetingModel model = new ShowMeetingModel
            {
                meeting = meeting,
                meetingPlaces = meetingPlaces,
                users = users
            };
            return View(model);
        }

        [RBAC]
        [HttpPost]
        public JsonResult Add_organizor(AddMeetingModel addMeetingModel)
        {
            
            if (ModelState.IsValid)
            {
                MeetingInfo meeting = addMeetingModel.meeting;
                MeetingService service = new MeetingService();
                DelegateService delegateService = new DelegateService();
                Status status = Status.SUCCESS;
                //调用服务
                if ((status = service.create(ref meeting)) == Status.SUCCESS)
                {
                    DeviceService deviceService = new DeviceService();
                    List<DeviceForDelegate> devices = null;
                    status = deviceService.getAllForDelegate(
                        meeting.meetingToStartTime,
                        meeting.meetingStartedTime,
                        out devices);
                    status = delegateService.createMultiple(devices,meeting.meetingID,addMeetingModel.delegates);
                    if (status != Status.SUCCESS)
                    {
                        List<int> meetingIDs = new List<int>();
                        meetingIDs.Add(meeting.meetingID);
                        service.deleteMultipe(meetingIDs);
                    }
                }
                return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(
               new RespondModel(
                   Status.ARGUMENT_ERROR,
                   ModelStateHelper.errorMessages(ModelState)),
                   JsonRequestBehavior.AllowGet);
            }
        }

        [RBAC]
        public ActionResult Show_organizor(int meetingID)
        {
            Status status = Status.SUCCESS;

            MeetingService meetingService = new MeetingService();
            MeetingInfo meeting = null;
            status = meetingService.getOne(meetingID, out meeting);

            AgendaService agendaService = new AgendaService();
            List<AgendaInfo> agendas = null;
            status = agendaService.getAll(meetingID, out agendas);

            DelegateService delegateService = new DelegateService();
            List<DelegateInfo> delegates = null;
            status = delegateService.getAll(meetingID, out delegates);

            MeetingPlaceService meetingPlaceService = new MeetingPlaceService();
            List<MeetingPlaceForMeeting> meetingPlaces = null;
            status = meetingPlaceService.getAllForMeeting(out meetingPlaces);

            ShowMeetingItemModel model = new ShowMeetingItemModel
            {
                meeting = meeting,
                agendas = agendas,
                delegates = delegates,
                meetingPlaces = meetingPlaces
            };

            Session["meetingID"] = meetingID;

            return View(model);
        }

        [HttpGet]
        [RBAC]
        public ActionResult Edit_organizor(int meetingID)
        {
            MeetingPlaceService meetingPlaceService = new MeetingPlaceService();
            List<MeetingPlaceForMeeting> meetingPlaces = null;
            meetingPlaceService.getAllForMeeting(out meetingPlaces);

            MeetingInfo meeting = null;
            MeetingService meetingService = new MeetingService();
            meetingService.getOne(meetingID, out meeting);

            EditMeetingModel model = new EditMeetingModel
            {
                meeting = meeting,
                meetingPlaces = meetingPlaces
            };

            Session["meetingID"] = meetingID;

            return View(model);
        }

        [RBAC]
        [HttpPost]
        public JsonResult Edit_organizor(MeetingInfo meeting)
        {
            //调用服务
            Status status = new MeetingService().update(meeting);

            return Json(new RespondModel(status,""), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [RBAC]
        public JsonResult Delete_organizor(List<int> meetingIDs)
        {
            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new MeetingService().deleteMultipe(meetingIDs);

            return Json(new RespondModel(status,""), JsonRequestBehavior.AllowGet);
        }
    }
}
