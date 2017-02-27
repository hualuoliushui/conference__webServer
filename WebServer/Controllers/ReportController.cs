using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DAL.DAO;
using DAL.DAOFactory;
using DAL.DAOVO;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using System.Data;
using System.Collections;
using System.Reflection;
using WebServer.Models.Meeting;
using WebServer.Models.Agenda;
using WebServer.Models.Report;
using WebServer.Models;
using WebServer.Models.Report.ReportInfo;

namespace WebServer.Controllers
{
    public class ReportController : Controller
    {
       
        [HttpGet]
        public ActionResult Index(int meetingID)
        {
             Status status = Status.SUCCESS;

             var service = new ReportService(meetingID);

             Session["meetingID"] = meetingID;

             return View(service.run());
        }
    }
}
