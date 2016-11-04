using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models
{
    public enum Status
    {
        FAILURE=-1,
        SUCCESS=0,
        FORMAT_ERROR=1,
        DATABASE_OPERATOR_ERROR=2,
        DATABASE_CONTENT_ERROR=3,
        NONFOUND=4
    }
}