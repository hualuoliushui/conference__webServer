using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebServer.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult PermissionDenied(string returnUrl)
        {
            Session["returnUrl"] = returnUrl;
            return View();
        }

    }
}
