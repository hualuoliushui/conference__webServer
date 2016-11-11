using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.Role;

namespace WebServer.Controllers
{
    public class RoleController : Controller
    {
        //
        // GET: /Role/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetRolesForUser()
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            List<RoleForUser> list = null;
            Status status = RoleService.getAllForUser(out list);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = list;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
