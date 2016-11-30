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

        public override String ToString()
        {
            return "{添加会议:" +
                "会议名称:" + meetingName +
                ",会场ID:" + meetingPlaceID +
                ",会议总结:" + meetingSummary +
                ",会议计划开始时间:" + meetingToStartTime +
                "}";
        }
    }
}