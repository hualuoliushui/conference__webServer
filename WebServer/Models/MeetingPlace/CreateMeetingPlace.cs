using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.MeetingPlace
{
    //提供给浏览器进行添加新会场的信息类
    public class CreateMeetingPlace
    {
        public string meetingPlaceName { set; get; }

        public int meetingPlaceCapacity { set; get; }


        public override String ToString()
        {
            return "{添加会场：" +
                "会场名称：" + meetingPlaceName +
                ",会场容量：" + meetingPlaceCapacity +
                "}";
        }
    }
}