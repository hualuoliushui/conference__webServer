using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.Vote;

namespace WebServer.Controllers
{
    public class VoteController : Controller
    {
        //
        // GET: /Vote/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetVotes(int agendaID)
        {
            RespondModel respond = new RespondModel();

            List<Vote> votes;
            //调用附件服务
            Status status =new VoteService().getAll(agendaID, out votes);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = votes;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteVoteMultipe(List<int> votes)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new VoteService().deleteMultipe(userName, votes);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateVote(UpdateVote vote)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new VoteService().update(userName, vote);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateVote(CreateVote vote)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new VoteService().create(userName, vote);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

    }
}
