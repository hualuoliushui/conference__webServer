using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Meeting
{
    public class Meeting
    {
        public int meetingID { set; get; }

        public string meetingName { set; get; }

        public string meetingPlaceName { set; get; }   

        public string meetingSummary { set; get; }

        public DateTime meetingToStartTime { set; get; }

        public int meetingStatus { set; get; }

        public override String ToString()
        {
            return "{会议:" +
                "会议ID:"+meetingID+
                ",会议名称:" + meetingName +
                ",会场:" + meetingPlaceName +
                ",会议总结:" + meetingSummary +
                ",会议计划开始时间:" + meetingToStartTime +
                ",会议状态:"+meetingStatus+
                "}";
        }
    }
}