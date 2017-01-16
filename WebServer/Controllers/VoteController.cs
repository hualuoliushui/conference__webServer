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
        [RBAC]
        public ActionResult Index_organizor(int agendaID)
        {
            RespondModel respond = new RespondModel();

            List<VoteInfo> votes;
            //调用附件服务
            Status status = new VoteService().getAll(agendaID, out votes);

            Session["agendaID"] = agendaID;
            return View(votes);
        }

        [HttpGet]
        [RBAC]
        public ActionResult Add_organizor(int agendaID)
        {
            Session["agendaID"] = agendaID;
            return View();
        }

        [HttpPost]
        [RBAC]
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
        [RBAC]
        public ActionResult Edit_organizor(int voteID)
        {
            VoteService voteService = new VoteService();
            VoteInfo vote = null;
            Status status = voteService.getOne(voteID, out vote);

            Session["agendaID"] = vote.agendaID;
            return View(vote);
        }

        [HttpPost]
        [RBAC]
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
        [RBAC]
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
