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

        public JsonResult GetDevices()
        {
            RespondModel respond = new RespondModel();

            Devices devices;
            //调用设备服务
            respond.Code = DeviceService.getAll(out devices);
            respond.Result = devices;

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

        public JsonResult CreateDevice(HttpContext context)
        {
            RespondModel respond = new RespondModel();
            //调用json解析
            Device device = Models.Tools.JsonHelper.GetObjectService<Device>(context);
            //调用设备服务
            respond.Code = DeviceService.create(device);

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

        public JsonResult UpdateDevices(HttpContext context)
        {
            RespondModel respond = new RespondModel();
            //调用json解析
            Devices devices = Models.Tools.JsonHelper.GetObjectService<Devices>(context);
            //调用设备服务
            respond.Code = DeviceService.update(devices);
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

        public JsonResult DeleteDevices(HttpContext context)
        {
            RespondModel respond = new RespondModel();
            //调用json解析
            OldDevices devices = Models.Tools.JsonHelper.GetObjectService<OldDevices>(context);
            //调用设备服务
            respond.Code = DeviceService.delete(devices);

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
