using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.App_Start;

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

            string system = form["system"];
            if(string.IsNullOrWhiteSpace(system))
                return View("Index");

            if (string.Compare(system, "admin") == 0)
            {
                return RedirectToAction("Admin");
            }
            if (string.Compare(system, "organizor") == 0)
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
            return View("Organizor");
        }
    }
}
