using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Meeting
{
    public class TipMeeting
    {
        public int meetingID { set; get; }

        public string meetingName { set; get; }

        public int meetingStatus { set; get; }
    }
}