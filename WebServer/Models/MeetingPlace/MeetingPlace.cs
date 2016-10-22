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

        public int meetingPlaceType { set; get; }

        public int meetingPlaceCapacity { set; get; }
    }
}