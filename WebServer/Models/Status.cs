using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models
{
    /// <summary>
    /// 表示返回结果的状态码，及描述
    /// </summary>
    public enum Status
    {
        FAILURE=-1,
        SUCCESS=0,
        FORMAT_ERROR=1,
        DATABASE_OPERATOR_ERROR=2,
        DATABASE_CONTENT_ERROR=3,
        NONFOUND=4,
        ARGUMENT_ERROR=5,
        PERMISSION_DENIED=6
    }
}