using System.Collections.Generic;
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
