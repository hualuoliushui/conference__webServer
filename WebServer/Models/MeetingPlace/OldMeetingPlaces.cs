using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.MeetingPlace
{
    public class OldMeetingPlaces
    {
        public List<int> meetingPlaces { set; get; }

        public static int deleteMeetingPlaces(OldMeetingPlaces oldMeetingPlaces)
        {
            //根据标识删除会场信息

            return 1;
        }
    }

}