using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.App_Start;
using WebServer.Models;
using WebServer.Models.Tools;
using WebServer.Models.Vote;

namespace WebServer.Controllers
{
    public class VoteController : Controller
    {
        [HttpGet]
        public ActionResult Index_organizor(int agendaID)
        {
            Session["agendaID"] = agendaID;

            RespondModel respond = new RespondModel();

            List<VoteInfo> votes;
            //调用附件服务
            Status status = new VoteService().getAll(agendaID, out votes);

            return View(votes);
        }

        [HttpGet]
        public ActionResult Add_organizor(int agendaID)
        {
            Session["agendaID"] = agendaID;
            return View();
        }

        [HttpPost]
        public JsonResult Add_organizor(CreateVote createVote)
        {
            Status status = Status.SUCCESS;

            if (ModelState.IsValid)
            {
                VoteService voteService = new VoteService();
                status = voteService.create(createVote);

                return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
            }
           
            return Json(
               new RespondModel(
                   Status.ARGUMENT_ERROR,
                   ModelStateHelper.errorMessages(ModelState)),
                   JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit_organizor(int voteID)
        {
            VoteService voteService = new VoteService();
            VoteInfo vote = null;
            Status status = voteService.getOne(voteID, out vote);

            if (status != Status.SUCCESS)
                return RedirectToAction("Error", "Error");

            return View(vote);
        }

        [HttpPost]
        public ActionResult Edit_organizor(UpdateVote updateAgenda)
        {
            Status status = Status.SUCCESS;

            if (ModelState.IsValid)
            {
                VoteService voteService = new VoteService();
                status = voteService.update(updateAgenda);
                return Json(new RespondModel(status, ""), JsonRequestBehavior.AllowGet);
            }

            return Json(
               new RespondModel(
                   Status.ARGUMENT_ERROR,
                   ModelStateHelper.errorMessages(ModelState)),
                   JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete_organizor(List<int> IDs)
        {
            Status status = Status.SUCCESS;

            VoteService voteService = new VoteService();
            status = voteService.deleteMultipe(IDs);

            return Json(
               new RespondModel(status, ""),
                   JsonRequestBehavior.AllowGet);
        }
    }
}
