using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using WebServer.App_Start;
using WebServer.Models;
using WebServer.Models.Delegate;

namespace WebServer.Controllers
{
    public class DelegateController : Controller
    {
        //
        // GET: /Delegate/

        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取指定会议的参会人员
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        [RBAC]
        public JsonResult GetDelegates(int meetingID)
        {
            RespondModel respond = new RespondModel();

            List<Models.Delegate.Delegate> delegates;

            Status status = new DelegateService().getAll(meetingID,out delegates);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = delegates;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加多个参会人员
        /// </summary>
        /// <param name="delegates"></param>
        /// <returns></returns>
        [RBAC]
        public JsonResult CreateDelegateMultipe(List<CreateDelegate> delegates)
        {
            RespondModel respond = new RespondModel();


            string userName = HttpContext.User.Identity.Name;
            Status status = new DelegateService().createMultiple(userName, delegates);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加单个参会人员
        /// </summary>
        /// <param name="createDelegate"></param>
        /// <returns></returns>
        [RBAC]
        public JsonResult CreateDelegate(CreateDelegate createDelegate)
        {
            RespondModel respond = new RespondModel();


            string userName = HttpContext.User.Identity.Name;
            Status status = new DelegateService().create(userName, createDelegate);

            respond.Code = (int)status;
            respond.Message = status.ToString();
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更新参会人员
        /// </summary>
        /// <param name="updateDelegate"></param>
        /// <returns></returns>
        [RBAC]
        public JsonResult UpdateDelegate(UpdateDelegate updateDelegate)
        {
            RespondModel respond = new RespondModel();


            string userName = HttpContext.User.Identity.Name;
            Status status = new DelegateService().update(userName, updateDelegate);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 批量删除参会人员
        /// </summary>
        /// <param name="delegates"></param>
        /// <returns></returns>
        [RBAC]
        public JsonResult DeleteDelegateMultipe(List<int> delegates)
        {
            RespondModel respond = new RespondModel();


            string userName = HttpContext.User.Identity.Name;
            Status status = new DelegateService().deleteMultipe(userName, delegates);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = "";

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
        [RBAC]
        public JsonResult GetSpeakerForAgenda(int meetingID)
        {
            RespondModel respond = new RespondModel();

            List<SpeakerForAgenda> speakers;
            Status status = new DelegateService().getSpeakerForAgenda(meetingID, out speakers);

            respond.Code = (int)status;
            respond.Message = Message.msgs[respond.Code];
            respond.Result = speakers;

            return Json(respond, JsonRequestBehavior.AllowGet);
        }
    }
}
