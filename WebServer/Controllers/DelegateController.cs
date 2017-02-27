using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebServer.App_Start;
using WebServer.Models;
using WebServer.Models.Delegate;
using WebServer.Models.Device;
using WebServer.Models.Meeting;
using WebServer.Models.Tools;
using WebServer.Models.User;

namespace WebServer.Controllers
{
    public class DelegateController : Controller
    {
        [HttpGet]
        [RBAC]
        public ActionResult Index_organizor(int meetingID)
        {
            Status status = Status.SUCCESS;

            MeetingService meetingService = new MeetingService();
            MeetingInfo meeting = null;

            status = meetingService.getOne(meetingID, out meeting);

            DeviceService deviceService = new DeviceService();
            List<DeviceForDelegate> devices = null;

            status = deviceService.getAllForDelegate(meeting.meetingToStartTime, meeting.meetingStartedTime, out devices);

            DelegateService delegateService = new DelegateService();
            List<DelegateInfo> delegates = null;

            status = delegateService.getAll(meetingID,out delegates);

            Session["meetingID"] = meetingID;
            return View(Tuple.Create(delegates,devices));
        }

        [HttpPost]
        [RBAC]
        public JsonResult Edit_organizor(UpdateDelegate model)
        {
            if (ModelState.IsValid)
            {
                Status status = Status.SUCCESS;

                DelegateService delegateServcie = new DelegateService();
                status = delegateServcie.update(model);

                return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
            }

            return Json(
                new RespondModel(
                    Status.ARGUMENT_ERROR,
                    ModelStateHelper.errorMessages(ModelState)),
                    JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [RBAC]
        public ActionResult Add_organizor(int meetingID)
        {
            Status status = Status.SUCCESS;

            MeetingService meetingService = new MeetingService();
            MeetingInfo meeting = null;

            status = meetingService.getOne(meetingID, out meeting);

            DeviceService deviceService = new DeviceService();
            List<DeviceForDelegate> devices = null;

            status = deviceService.getAllForDelegate(meeting.meetingToStartTime, meeting.meetingStartedTime, out devices);

            UserService userService = new UserService();
            List<UserForDelegate> users = null;

            status = userService.getNewForDelegate(meetingID, out users);

            Session["meetingID"] = meetingID;

            return View(Tuple.Create(users, devices));
        }

        [HttpPost]
        [RBAC]
        public JsonResult Add_organizor(CreateDelegate model)
        {
            Status status = Status.SUCCESS;

            if (ModelState.IsValid)
            {
                DelegateService delegateService = new DelegateService();
                status = delegateService.create(model);

                return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
            }

            return Json(
                new RespondModel(
                    Status.ARGUMENT_ERROR, 
                    ModelStateHelper.errorMessages(ModelState)), 
                    JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        [RBAC]
        public JsonResult Delete_organizor(List<int> delegateIDs)
        {
            Status status = Status.SUCCESS;

            DelegateService delegateService = new DelegateService();
            status = delegateService.deleteMultipe(delegateIDs);
            return Json(new RespondModel(status,""), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SeatArrange_organizor(int meetingID)
        {
            Session["meetingID"] = meetingID;

            DelegateService delegateService = new DelegateService();

            int meetingPlaceID = -1;
            var seatType = delegateService.getSeatType(meetingID,out meetingPlaceID);

            if (meetingPlaceID == -1)
            {
                throw new System.Exception("会场不存在");
            }
            
            switch (seatType)
            {
                case 0:
                    return RedirectToAction("SeatArrange_LongTable_organizor", new { meetingID = meetingID, meetingPlaceID = meetingPlaceID });
                    break;
                default:
                    return RedirectToAction("SeatArrange_LongTable_organizor", new { meetingID = meetingID, meetingPlaceID = meetingPlaceID });
                    break;
            }
        }

        [HttpGet]
        public ActionResult SeatArrange_LongTable_organizor(int meetingID, int meetingPlaceID)
        {
            DelegateService service = new DelegateService();

            return View(service.getSeatInfos_LongTable(meetingID, meetingPlaceID));
        }

        [HttpPost]
        public JsonResult SeatArrange_organizor(List<SeatInfo> seatInfos)
        {
            DelegateService delegateService = new DelegateService();
            Status status = delegateService.updateSeatInfos(seatInfos);

            return Json(new RespondModel(status,""),JsonRequestBehavior.AllowGet);
        }
    }
}
