using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models
{
    /// <summary>
    /// 封装返回结果的对象
    /// </summary>
    public class RespondModel
    {
        public int Code { set; get; }
        public string Message { set; get; }
        public object Result { set; get; }
    }
}