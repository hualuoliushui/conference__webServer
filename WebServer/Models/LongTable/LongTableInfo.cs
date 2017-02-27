using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.LongTable
{
    public class LongTableInfo
    {
        public int longTableID { set; get; }

        public int meetingPlaceID { set; get; }

        public int upNum { set; get; }

        public int downNum { set; get; }

        public int leftNum { set; get; }

        public int rightNum { set; get; }
    }
}