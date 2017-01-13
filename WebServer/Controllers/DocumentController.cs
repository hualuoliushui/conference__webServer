using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WebServer.App_Start;
using WebServer.Models;
using WebServer.Models.Document;
using WebServer.Models.Document.FileConvertService;

namespace WebServer.Controllers
{
    public class DocumentController : Controller
    {
        [HttpGet]
        [RBAC]
        public ActionResult Index_organizor(int agendaID)
        {
            RespondModel respond = new RespondModel();

            List<Document> documents;
            //调用附件服务
            Status status = new DocumentService().getAll(agendaID, out documents);

            Session["agendaID"] = agendaID;
            return View(documents);
        }

        [HttpGet]
        [RBAC]
        public ActionResult Add_organizor(int agendaID){
            Session["agendaID"] = agendaID;
            return View();
        }

        [HttpPost]
        [RBAC]
        public JsonResult Add_organizor()
        {
            Status status = Status.SUCCESS;//初始化为SUCCESS

            try
            {
                do
                {
                    var form = Request.Form;
                    if (form == null || form["agendaID"] == null)
                    {
                        status = Status.ARGUMENT_ERROR;
                        break;
                    }
                    int agendaID = Int32.Parse(form["agendaID"]);//从浏览器得到agendaID
                    //初始化附件服务
                    DocumentService service = new DocumentService();

                    status = service.upFile(Request.Files, agendaID);

                } while (false);

            }
            catch (Exception e)
            {
                Log.LogInfo("文件上传", e);  
            }

            return Json(new RespondModel(status,""), JsonRequestBehavior.AllowGet);
        }

        [RBAC]
        public JsonResult StartConvert(int agendaID)
        {
            //调用服务
            Status status = new DocumentService().convertFile(agendaID);

            return Json(new RespondModel(status,""), JsonRequestBehavior.AllowGet);
        }

        //通过服务器相对路径进行文件下载，已完成----李杭澍
        [RBAC]
        [HttpGet]
        public void Download(string fileName)
        {
            Response.ContentType = "application/x-zip-compressed";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);//以附件形式下载
            string SaveDir = ConfigurationManager.AppSettings["SaveDir"];
            string fileFullPath = SaveDir + fileName;//文件存储在服务器的相对路径
            if (System.IO.File.Exists(fileFullPath))
            {
                Response.TransmitFile(fileFullPath);
            }
        }

        [HttpPost]
        [RBAC]
        public JsonResult Delete_organizor(List<int> IDs)
        {
            Status status = Status.SUCCESS;

            DocumentService documentService = new DocumentService();
            status = documentService.deleteMultipe(IDs);

            return Json(
               new RespondModel(status, ""),
                   JsonRequestBehavior.AllowGet);
        }
    }
}
