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
            HttpContext content =  HttpContext.Current;

            do
            {
                if (content == null || content.Session["user"] == null)
                {
                    break;
                }

                RBACUser requestingUser = (RBACUser)content.Session["user"];

                if (!requestingUser.HasPermission(requiredPermission))
                {
                    break;
                }
                base.OnAuthorization(filterContext);
                return;
            } while (false);
       
            base.OnAuthorization(filterContext);
            filterContext.Result = new RedirectToRouteResult(
                             new System.Web.Routing.RouteValueDictionary{
                        {"action","Index"},
                        {"controller","Account"}
                    });
        }
    }
}