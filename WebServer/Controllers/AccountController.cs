using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.App_Start;
using WebServer.Models.Meeting;

namespace WebServer.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            string userName = form["userName"];
            string password = form["password"];
            if (!Forms.Verigy(userName, password))
            {
                return RedirectToAction("Index");
            }

            Forms.Login(userName, 20);

            Session["userName"] = userName;

            int system = 1;
            
            if(string.IsNullOrWhiteSpace(form["system"]))
                return View("Index");

            system = Int32.Parse(form["system"]);

            if ( system == 0)
            {
                return RedirectToAction("Admin");
            }
            if (system == 1)
            {
                return RedirectToAction("Organizor");
            }

            return View("Index");
        }

        [RBAC]
        [HttpGet]
        public ActionResult Admin()
        {
            return View("Admin");
        }

        [RBAC]
        [HttpGet]
        public ActionResult Organizor()
        {
            List<TipMeeting> meetings = null;
            //调用服务
            Status status = new MeetingService().getAll(out meetings);

            return View(meetings);
        }
    }
}
