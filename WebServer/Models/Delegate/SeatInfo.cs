using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Delegate
{
    public class SeatInfo
    {
        public int delegateID { set; get; }
        public string userName { set; get; }
        public int deviceIndex { set; get; }
        public int seatIndex { set; get; }
        public int userLevel { set; get; }
    }
}