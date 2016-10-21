using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models
{
    public class RespondModel
    {
        public int Code { set; get; }
        public string Message { set; get; }
        public object Result { set; get; }
    }
}