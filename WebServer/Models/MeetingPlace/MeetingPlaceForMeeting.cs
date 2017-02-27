using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.MeetingPlace
{
    public class MeetingPlaceForMeeting
    {
        public int meetingPlaceID { set; get; }
        public string meetingPlaceName { set; get; }
        public int seatType { set; get; }

        public override String ToString()
        {
            return "{会场：" +
                "会场ID：" + meetingPlaceID +
                ",会场名称：" + meetingPlaceName +
                ",座位类型：" + seatType +
                "}";
        }
    }
}