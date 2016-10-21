using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.MeetingPlace
{
    public class MeetingPlaces
    {
        public List<MeetingPlace> meetingPlaces;

        public static int getMeetingPlaces(out MeetingPlaces meetingPlaces)
        {
            //查询数据
            meetingPlaces = new MeetingPlaces();
            meetingPlaces.meetingPlaces = new List<MeetingPlace>();

            meetingPlaces.meetingPlaces.Add(new MeetingPlace { meetingPlaceID = 1, meetingPlaceName = "人民大会堂", meetingPlaceType = "主席台型", meetingPlaceCapacity = 200 });
            meetingPlaces.meetingPlaces.Add(new MeetingPlace { meetingPlaceID = 2, meetingPlaceName = "学术大讲堂", meetingPlaceType = "宾客来访型", meetingPlaceCapacity = 100 });
      
            return 1;
        }

       
    }
}