using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.MeetingPlace
{
    public class NewMeetingPlace
    {
        public string meetingPlaceName { set; get; }
        public string meetingPlaceType { set; get; }
        public int meetingPlaceCapacity { set; get; }

        public static int createMeetingPlace(NewMeetingPlace newMeetingPlace)
        {
            //执行数据库插入操作

            return 1;
        }
    }
}