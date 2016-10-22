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
            //调用会场服务
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
            DeviceVO device = Models.Tools.JsonHelper.GetObjectService<DeviceVO>(context);
            //调用会场服务
            respond.Code = DeviceService.create(newMeetingPlac);

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
            //调用会场服务
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
            OleDevices devices = Models.Tools.JsonHelper.GetObjectService<OleDevices>(context);
            //调用会场服务
            respond.Code = DeviceService.delete(devices);
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
