using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Meeting
{
    public class NewMeeting
    {
        public int meetingPlaceID { set; get; }

        public string meetingName { set; get; }

        public string meetingSummary { set; get; }

        public DateTime meetingStartedTime { set; get; }
    }
}