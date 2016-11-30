using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using WebServer.Models;
using WebServer.Models.User;

namespace WebServer.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        [HttpGet]
        public ActionResult Index_admin()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Add_admin()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit_admin()
        {
            return View();
        }

        public JsonResult GetUsersForDelegate()
        {
            RespondModel respond = new RespondModel();
            //调用会场服务
            List<UserForDelegate> list = null;
            Status status = new UserService().getAllForDelegate(out list);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = list;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUsers()
        {
            RespondModel respond = new RespondModel();

            List<User> users;
            //调用用户服务
            Status status = new UserService().getAll(out users);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = users;

           return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUserForUpdate(int userID)
        {
            RespondModel respond = new RespondModel();

            UpdateUser user;
            //调用用户服务
            Status status = new UserService().getOneUpdate(out user, userID);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = user;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Upload(FormCollection form)
        {
            RespondModel respond = new RespondModel();
            Status status = Status.SUCCESS;//初始化为SUCCESS
            List<Status> checkList = new List<Status>();//导入数据结果清单

            //初始化附件服务
            UserService userService = new UserService();

            //获取需要导入的表格名称
            String tableName = form["tableName"];
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = "Sheet1";//如果无输入，则默认为Sheet1
            }
            //上传后文件存储在服务器端的路径
            string filePath = System.Web.HttpContext.Current.Server.MapPath(@"\temp\");//注意：议程不存在时文件路径为空，会报FAILURE
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            if (filePath != null && Request.Files["file"].ContentLength > 0)//文件路径存在和文件不为空时上传文件
            {
                string fileName = Request.Files["file"].FileName;
                string fileFullPath = filePath + fileName;//文件路径+文件名

                //判断文件的后缀是否符合要求(excel类文件)
                string Extension = System.IO.Path.GetExtension(fileName);
                if (
                    Extension == ".xls" || Extension == ".xlsx")
                {
                    Request.Files["file"].SaveAs(fileFullPath);//上传

                    FileInfo fi = new FileInfo(fileFullPath);

                    string userName = HttpContext.User.Identity.Name;//登录时的用户名
                    //将文件信息写入数据库
                    status = userService.createMultiple(fileFullPath,tableName,ref checkList);
                    fi.Delete();
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
            respond.Result = checkList;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateUser(CreateUser user)
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            Status status = new UserService().create(user);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CreateForDelegate(CreateUserForDelegate user)
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            Status status = new UserService().createForDelegate(user);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUser(UpdateUser user)
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            Status status = new UserService().update(user);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUserAvailable(int userID, int state)//state对应数据库中的available字段
        {
            RespondModel respond = new RespondModel();
            //调用用户服务
            Status status = new UserService().UpdateUserAvailable(userID, state);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
