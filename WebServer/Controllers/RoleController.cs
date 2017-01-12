using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebServer.App_Start;
using WebServer.Models;
using WebServer.Models.Role;

namespace WebServer.Controllers
{
    public class RoleController : Controller
    {
        //
        // GET: /Role/

        public ActionResult Index_admin()
        {
            return View();
        }

        /// <summary>
        /// 为用户赋予角色
        /// </summary>
        /// <returns></returns>
        //[RBAC]
        public JsonResult GetRolesForUser()
        {
            RespondModel respond = new RespondModel();
            //调用角色服务
            List<RoleForUser> list = null;
            Status status = new RoleService().getAllForUser(out list);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = list;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 为角色赋予权限
        /// </summary>
        /// <returns></returns>
        [RBAC]
        public JsonResult GetPermissions()
        {
            RespondModel respond = new RespondModel();
            //调用角色服务
            List<Permission> list = null;
            Status status = new RoleService().getPermissions(out list);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = list;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [RBAC]
        public JsonResult GetRoles()
        {
            RespondModel respond = new RespondModel();
            //调用角色服务
            Roles roles;
            Status status = new RoleService().getAll(out roles);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = roles;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [RBAC]
        public JsonResult CreateRole(CreateRole role)
        {
            RespondModel respond = new RespondModel();
            //调用角色服务
            Status status = new RoleService().create(role);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
