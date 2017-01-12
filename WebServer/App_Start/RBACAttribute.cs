using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebServer.Models;
using System.Web.Mvc;

namespace WebServer.App_Start
{
    public class RBACAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            /*Create hasPermission string based on the requested controller 
             name and action name in the format 'controllername-action'*/
            string requiredPermission = String.Format("{0}-{1}",
                   filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                   filterContext.ActionDescriptor.ActionName);
            //filterContext.RequestContext.HttpContext.User.Identity.Name
            RBACUser requestingUser = new RBACUser("测试");

            if (!requestingUser.HasPermission(requiredPermission))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary{
                        {"action","Index"},
                        {"controller","Account"}
                    });
            }
            base.OnAuthorization(filterContext);
        }
    }
}