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

namespace WebServer.Controllers
{
    public class ReportController : Controller
    {
         

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public void Export(int meetingID)
        {
             var service = new ReportService(meetingID);
             var pdfFileName = service.Export();

            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();

            Response.ContentType = "application/x-zip-compressed";
            Response.AddHeader("Content-Disposition", "attachment;filename="
                + pdfFileName);//以附件形式下载
            if (System.IO.File.Exists(service.pdfFullName))
            {
                Response.TransmitFile(service.pdfFullName);
            } 
        }
    }
}
