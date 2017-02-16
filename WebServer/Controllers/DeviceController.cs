using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.Device;
using DAL.DAOVO;
using WebServer.App_Start;
using WebServer.Models.Tools;

namespace WebServer.Controllers
{
    public class DeviceController : Controller
    {
        //
        // GET: /Device/

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
        public JsonResult GetDevices()
        {
            RespondModel respond = new RespondModel();

            List<Device> devices;
            //调用设备服务
            Status status = new DeviceService().getAll(out devices);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = devices;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
   
        [RBAC]
        public JsonResult GetDeviceForUpdate(int deviceID)
        {
            RespondModel respond = new RespondModel();

            UpdateDevice device;
            //调用设备服务
            Status status = new DeviceService().getOneForUpdate(out device, deviceID);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = device;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
        [RBAC]
        public JsonResult CreateDevice(CreateDevice device)
        {
            if (ModelState.IsValid)
            {
                Status status = new DeviceService().create(device);

                return Json(new RespondModel(status,""), JsonRequestBehavior.AllowGet);
            }

            return Json(new RespondModel(Status.ARGUMENT_ERROR, ModelStateHelper.errorMessages(ModelState)), JsonRequestBehavior.AllowGet);
        }

        [RBAC]
        public JsonResult UpdateDevice(UpdateDevice device)
        {
            if (ModelState.IsValid)
            {
                //调用设备服务
                Status status = new DeviceService().update(device);
                return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
            }
            return Json(new RespondModel(Status.ARGUMENT_ERROR, ModelStateHelper.errorMessages(ModelState)), JsonRequestBehavior.AllowGet);
        }
        [RBAC]
        public JsonResult UpdateDeviceAvailable(int deviceID, int deviceFreezeState)//FreezeState对应数据库中的available字段
        {
            RespondModel respond = new RespondModel();
            //调用设备服务
            Status status = new DeviceService().UpdateDeviceAvailable(deviceID, deviceFreezeState);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
        
    }
}
