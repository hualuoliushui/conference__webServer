using System.Web.Mvc;
using System.Collections.Generic;
using WebServer.App_Start;
using WebServer.Models;
using WebServer.Models.LongTable;
using WebServer.Models.Tools;

namespace WebServer.Controllers
{
    public class LongTableController : Controller
    {
        public JsonResult CreateLongTable(CreateLongTable longTable)
        {
            if (ModelState.IsValid)
            {
                //调用会场服务
                Status status = new LongTableService().create(longTable);
                return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
            }

            return Json(new RespondModel(Status.ARGUMENT_ERROR, ModelStateHelper.errorMessages(ModelState)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLongTableForUpdate(int meetingPlaceID)
        {
            RespondModel respond = new RespondModel();

            UpdateLongTable longTable;
            //调用设备服务
            Status status = new LongTableService().getOneForUpdate(meetingPlaceID, out longTable);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = longTable;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

    }
}
