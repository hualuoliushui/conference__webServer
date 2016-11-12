using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using System.Diagnostics;
using System.Net.Mail;

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


            //Exception error = Server.GetLastError().GetBaseException();
            ////在事件日志中记录异常  
            //if (!EventLog.SourceExists("ApplicationException"))
            //{
            //    EventLog.CreateEventSource("ApplicationException", "Application");
            //}

            //EventLog eventLog = new EventLog();
            //eventLog.Log = "Application";
            //eventLog.Source = "ApplicationException";
            //eventLog.WriteEntry(error.ToString(), EventLogEntryType.Error);

            ////发送Email给开发人员  
            //MailMessage email = new MailMessage("115089190@qq.com", "115089190@qq.com");
            //email.Body = error.ToString();
            //email.IsBodyHtml = true;
            //email.Subject = "An error occurred in the  Application";
            //SmtpClient smtpClient = new SmtpClient("stmp.qq.com", 465);
            //smtpClient.Credentials = new System.Net.NetworkCredential("115089190@qq.com", "qq.5g5a5n");
            //smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //smtpClient.Send(email);
            //Response.Redirect("/Error/Index");
        }
    }
}