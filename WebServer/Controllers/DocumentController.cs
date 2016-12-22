using System;
using System.Collections.Generic;
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

        public ActionResult Index()
        {
            return View();
        }

        //[RBAC]
        [HttpPost]
        public JsonResult Upload(FormCollection form)
        {
            RespondModel respond = new RespondModel();
            Status status = Status.SUCCESS;//初始化为SUCCESS

            try
            {
                do
                {
                    if (form["agendaID"] == null)
                    {
                        respond.Code = (int)Status.ARGUMENT_ERROR;
                    }
                    int agendaID = Int32.Parse(form["agendaID"]);//从浏览器得到agendaID
                    //初始化附件服务
                    DocumentService documentService = new DocumentService();
                    //上传后文件存储在服务器端的路径,及相对路径
                    string srcFileRelativeDirectory_Origin = null;
                    string srcFileAbsoluteDirectory = documentService.getSrcFileAbsoluteDirectory(agendaID, out srcFileRelativeDirectory_Origin);//注意：议程不存在时文件路径为空，会报FAILURE

                    if (srcFileAbsoluteDirectory != null && srcFileRelativeDirectory_Origin != null && Request.Files["file"].ContentLength > 0)//文件路径存在和文件不为空时上传文件
                    {
                        string fileName = Request.Files["file"].FileName;
                        string srcFileAbsolutePath = srcFileAbsoluteDirectory + fileName;//文件路径+文件名


                        //判断文件的后缀是否符合要求(word、excel、ppt三类文件)
                        string Extension = System.IO.Path.GetExtension(fileName);
                        if (Extension == ".doc" || Extension == ".docx" ||
                            Extension == ".xls" || Extension == ".xlsx" ||
                            Extension == ".ppt" || Extension == ".pptx")
                        {
                            Request.Files["file"].SaveAs(srcFileAbsolutePath);//上传

                            FileInfo fi = new FileInfo(srcFileAbsolutePath);
                            long fileSize = fi.Length / 1024;//文件大小的单位是KB

                            string userName = HttpContext.User.Identity.Name;//登录时的用户名
                            //将文件信息写入数据库的操作失败时，删除已上传的相应文件
                            if (documentService.addFile(userName, agendaID, fileName, fileSize, srcFileRelativeDirectory_Origin) != Status.SUCCESS)
                            {
                                fi.Delete();
                                status = Status.FAILURE;
                            }
                            else
                            {
                                #region 文件转换
                                //获取目标文件绝对路径名，和目标文件相对指定root目录的路径名
                                string targetFileRelativeDirectory_Root = null;
                                string targetFileAbsolutePath = documentService.getTargetFileAbsolutePath(srcFileAbsolutePath, srcFileRelativeDirectory_Origin, out targetFileRelativeDirectory_Root);
                                //文件转换开始
                                status = documentService.convertFile(srcFileAbsolutePath, targetFileAbsolutePath, targetFileRelativeDirectory_Root);
                                #endregion
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

                } while (false);
            }
            catch (Exception e)
            {
                Log.LogInfo("文件上传", e);
                respond.Code = (int)Status.FAILURE;
                respond.Message = Message.msgs[respond.Code];
                respond.Result = "";
                return Json(respond, JsonRequestBehavior.AllowGet);
            }
            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        //通过服务器相对路径进行文件下载，已完成----李杭澍
        [RBAC]
        [HttpGet]
        public void Download(string fileName)
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
        [RBAC]
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
        [RBAC]
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
