using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.Document;

namespace WebServer.Controllers
{
    public class DocumentController : Controller
    {
        //
        // GET: /Document/

        public ActionResult Index()
        {
            return View();
        }


        //上传文件,已完成----李杭澍
        [System.Web.Http.HttpPost]
        public JsonResult Upload(FormCollection form)
        {
            RespondModel respond = new RespondModel();
            Status status = Status.SUCCESS;//初始化为SUCCESS

            int agendaID = Int32.Parse(form["agendaID"]);//从浏览器得到agendaID
            //初始化附件服务
            DocumentService documentService = new DocumentService();
            //上传后文件存储在服务器端的路径
            string filePath = documentService.getFilePath(agendaID);//注意：议程不存在时文件路径为空，会报FAILURE

            //int meetingID = 2;
            ////上传后文件存储在服务器端的路径
            //string filePath = System.Web.HttpContext.Current.Server.MapPath(@"\upfiles\origin\" + meetingID + "\\" + agendaID + "\\");
            //if (!Directory.Exists(filePath))
            //{
            //    Directory.CreateDirectory(filePath);
            //}

            if (filePath != null && Request.Files["file"].ContentLength > 0)//文件路径存在和文件不为空时上传文件
            {
                string fileName = Request.Files["file"].FileName;
                string fileFullPath = filePath + fileName;//文件路径+文件名


                //判断文件的后缀是否符合要求(word、excel、ppt三类文件)
                string Extension = System.IO.Path.GetExtension(fileName);
                if (Extension == ".doc" || Extension == ".docx" ||
                    Extension == ".xls" || Extension == ".xlsx" ||
                    Extension == ".ppt" || Extension == ".pptx")
                {
                    Request.Files["file"].SaveAs(fileFullPath);//上传

                    FileInfo fi = new FileInfo(fileFullPath);
                    long fileSize = fi.Length / 1024;//文件大小的单位是KB

                    string userName = HttpContext.User.Identity.Name;//登录时的用户名
                    //将文件信息写入数据库的操作失败时，删除已上传的相应文件
                    if (documentService.addFile(userName, agendaID, fileName, fileSize, fileFullPath) == Status.FAILURE)
                    {
                        fi.Delete();
                        status = Status.FAILURE;
                    }
                    else
                    {
                        status = Status.SUCCESS;
                    }

                }
                else
                {
                    status = Status.FILE_NOT_SUPPORT;
                }
            }
            else
            {
                status = Status.FILE_PATH_ERROR;
            }

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        //通过服务器相对路径进行文件下载，已完成----李杭澍
        [System.Web.Http.HttpGet]
        public void Downloadtest(string fileName)
        {
            //documentPath = "test.docx";
            Response.ContentType = "application/x-zip-compressed";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);//以附件形式下载
            string fileFullPath = Server.MapPath(@"\upfiles\origin\" + fileName);//文件存储在服务器的相对路径
            if (System.IO.File.Exists(fileFullPath))
            {
                Response.TransmitFile(fileFullPath);
            }

        }

        public JsonResult GetDocuments(int agendaID)
        {
            RespondModel respond = new RespondModel();

            List<Document> documents;
            //调用附件服务
            Status status = new DocumentService().getAll(agendaID,out documents);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = documents;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDocumentMultipe(List<int> documents)
        {
            RespondModel respond = new RespondModel();

            //调用服务
            string userName = HttpContext.User.Identity.Name;
            Status status = new DocumentService().deleteMultipe(userName, documents);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
