using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Meeting
{
    public class EditMeetingModel
    {
        public MeetingInfo meeting { set; get; }
        public List<MeetingPlace.MeetingPlaceForMeeting> meetingPlaces { set; get; }
    }
}