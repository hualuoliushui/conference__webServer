using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;

namespace WebServer.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

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
                RedirectToAction("Index");
            }
            Forms.Login(userName, 20);
            return RedirectToAction("Index", "Admin", new { userName = userName });
        }

        public ActionResult Admin()
        {
            ViewData["organizors"] = 15;
            return View("Admin");
        }

        public ActionResult Organizor()
        {
            return View("Organizor");
        }
    }
}
