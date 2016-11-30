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

        public override String ToString()
        {
            return "{会议:" +
                "会议ID:" + meetingID +
                ",会议名称:" + meetingName +
                ",会议状态:" + meetingStatus +
                "}";
        }
    }
}