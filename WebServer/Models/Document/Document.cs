using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Document
{
    public class Document
    {
        public int documentID { set; get; }
        public string documentName { set; get; }
        public string documentSize { set; get; }
        public string documentPath { set; get; }
        public int agendaID { set; get; }
    }
}