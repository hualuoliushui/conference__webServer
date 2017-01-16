using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebServer.App_Start;
using WebServer.Models;
using WebServer.Models.Agenda;
using WebServer.Models.Delegate;
using WebServer.Models.Tools;

namespace WebServer.Controllers
{
    public class AgendaController : Controller
    {
        [HttpGet]
        [RBAC]
        public ActionResult Index_organizor(int meetingID)
        {
            AgendaService agendaService = new AgendaService();
            List<AgendaInfo> agendas = null;
            Status status = agendaService.getAll(meetingID, out agendas);

            Session["meetingID"] = meetingID;

            return View(agendas);
        }

        [HttpGet]
        [RBAC]
        public ActionResult Add_organizor(int meetingID)
        {
            DelegateService delegateService = new DelegateService();
            List<SpeakerForAgenda> speakersForAgenda = null;
            Status status = delegateService.getSpeakerForAgenda(meetingID, out speakersForAgenda);

            Session["meetingID"] = meetingID;

            return View(Tuple.Create(meetingID, speakersForAgenda));
        }

        [HttpPost]
        [RBAC]
        public JsonResult Add_organizor(CreateAgenda createAgenda)
        {
            Status status = Status.SUCCESS;

            if (ModelState.IsValid)
            {
                AgendaService agendaService = new AgendaService();
                status = agendaService.create(createAgenda);

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
        public ActionResult Edit_organizor(int agendaID)
        {
            AgendaService agendaService = new AgendaService();
            AgendaInfo agenda = null;
            Status status = agendaService.getOne(agendaID, out agenda);

            DelegateService delegateService = new DelegateService();
            List<SpeakerForAgenda> speakersForAgenda = null;
            status = delegateService.getSpeakerForAgenda(agenda.meetingID, out speakersForAgenda);

            Session["meetingID"] = agenda.meetingID;

            return View(Tuple.Create(agenda, speakersForAgenda));
        }

        [HttpPost]
        [RBAC]
        public JsonResult Edit_organizor(UpdateAgenda updateAgenda)
        {
            Status status = Status.SUCCESS;

            if (ModelState.IsValid)
            {
                AgendaService agendaService = new AgendaService();
                status = agendaService.update(updateAgenda);

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
        public JsonResult Delete_organizor(List<int> IDs)
        {
            Status status = Status.SUCCESS;

            AgendaService agendaService = new AgendaService();
            status = agendaService.deleteMultipe(IDs);

            return Json(
               new RespondModel(status,""),
                   JsonRequestBehavior.AllowGet);
        }
    }
}
