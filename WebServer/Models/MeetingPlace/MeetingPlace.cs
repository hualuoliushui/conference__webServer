using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.MeetingPlace
{
    //提供给浏览器的会场信息类
    public class MeetingPlace
    {
        public int meetingPlaceID { set; get; }

        public string meetingPlaceName { set; get; }

        public int meetingPlaceCapacity { set; get; }

        public int meetingPlaceFreezeState { set; get; }

        public override String ToString()
        {
            return "{会场：" +
                "会场ID：" + meetingPlaceID +
                ",会场名称：" + meetingPlaceName +
                ",会场容量：" + meetingPlaceCapacity +
                ",会场状态：" + meetingPlaceFreezeState +
                "}";
        }
    }
}