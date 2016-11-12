using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.Device;
using DAL.DAOVO;

namespace WebServer.Controllers
{
    public class DeviceController : Controller
    {
        //
        // GET: /Device/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public JsonResult GetDevices()
        {
            RespondModel respond = new RespondModel();

            List<Device> devices;
            //调用设备服务
            Status status = DeviceService.getAll(out devices);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = devices;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDevicesForDelegate()
        {
            RespondModel respond = new RespondModel();

            List<DeviceForDelegate> devices;
            //调用设备服务
            Status status = DeviceService.getAllForDelegate(out devices);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = devices;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDeviceForUpdate(string deviceID)
        {
            RespondModel respond = new RespondModel();

            UpdateDevice device;
            //调用设备服务
            Status status = DeviceService.getOneForUpdate(out device, deviceID);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = device;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateDevice(Device device)
        {
            RespondModel respond = new RespondModel();
            //调用设备服务
            Status status = DeviceService.create(device);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDevice(UpdateDevice device)
        {
            RespondModel respond = new RespondModel();
            //调用设备服务
            Status status = DeviceService.update(device);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDeviceAvailable(string deviceID, int deviceFreezeState)//FreezeState对应数据库中的available字段
        {
            RespondModel respond = new RespondModel();
            //调用设备服务
            Status status = DeviceService.UpdateDeviceAvailable(deviceID, deviceFreezeState);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
        
    }
}
