using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Delegate
{
    public class SeatArrange_LongTableModel
    {
        public int upNum { set; get; }
        public int downNum { set; get; }
        public int leftNum { set; get; }
        public int rightNum { set; get; }

        public List<SeatInfo> seatInfos { set; get; }
    }

    public class SeatInfo
    {
        public int delegateID { set; get; }
        public string userName { set; get; }
        public int deviceIndex { set; get; }
        public int seatIndex { set; get; }
        public int userLevel { set; get; }
    }
}