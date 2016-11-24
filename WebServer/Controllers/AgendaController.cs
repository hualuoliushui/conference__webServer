using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.Agenda;

namespace WebServer.Controllers
{
    public class AgendaController : Controller
    {
        //
        // GET: /Agenda/

        public ActionResult Index()
        {
            return View();
        }

         /// <summary>
         /// 获取指定会议的议程
         /// </summary>
         /// <param name="meetingID"></param>
         /// <returns></returns>
        public JsonResult GetAgendas(int meetingID)
        {
            RespondModel respond = new RespondModel();

            List<Agenda> agendas;

            Status status = new AgendaService().getAll(meetingID,out agendas);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = agendas;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 创建议程
        /// </summary>
        /// <param name="agenda"></param>
        /// <returns></returns>
        public JsonResult CreateAgenda(CreateAgenda agenda)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new AgendaService().create(userName, agenda);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAgena(UpdateAgenda agenda)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new AgendaService().update(userName, agenda);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAgendaMultipe(List<int> agendas)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new AgendaService().deleteMultipe(userName, agendas);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
