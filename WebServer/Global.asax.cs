using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebServer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_Error(object sender, EventArgs e)
        {
            //获取到HttpUnhandledException异常，这个异常包含一个实际出现的异常
            Exception ex = Server.GetLastError();
            //实际发生的异常
            Exception iex = ex.InnerException;

            string errorMsg = String.Empty;
            string particular = String.Empty;
            if (iex != null)
            {
                errorMsg = iex.Message;
                particular = iex.StackTrace;
            }
            else
            {
                errorMsg = ex.Message;
                particular = ex.StackTrace;
            }
            HttpContext.Current.Response.Write("来自Global的错误处理<br />");
            HttpContext.Current.Response.Write(errorMsg);

            Server.ClearError();//处理完及时清理异常
        }
    }
}