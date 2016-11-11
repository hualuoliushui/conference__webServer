using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.MeetingPlace
{
    //提供给浏览器的删除会场的标识列表类
    public class UpdateMeetingPlace
    {
        public int meetingPlaceID { set; get; }

        public string meetingPlaceName { set; get; }

        public int meetingPlaceCapacity { set; get; }
    }
}