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

        public ActionResult Index()
        {
            return View();
        }
        public class student
        {
            public int userID { set; get; }
            public string userName { set; get; }
            public List<student> list { set; get; }
        }
        public JsonResult testPost(student s)
        {
            return Json(s, JsonRequestBehavior.AllowGet);
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
            Session["logined"] = "logined";
            Forms.Login(userName, 20);
            return RedirectToAction("Admin");
        }

        public ActionResult Admin()
        {
            return View("Admin");
            if (Session["logined"] == null)
            {
                return View("Index");
            }
            return View("Admin");
        }

        [RBAC]
        public ActionResult Organizor()
        {
            if (Session["logined"] == null)
            {
                return View("Index");
            }
            return View("Organizor");
        }
    }
}
