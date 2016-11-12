using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Meeting
{
    public class CreateMeeting
    {
        public string meetingName { set; get; }

        public int meetingPlaceID { set; get; }

        public string meetingSummary { set; get; }

        public DateTime meetingToStartTime { set; get; }
    }
}