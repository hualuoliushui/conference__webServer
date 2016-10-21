using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.MeetingPlace
{
    public class MeetingPlace
    {
        public int meetingPlaceID { set; get; }
        public string meetingPlaceName { set; get; }
        public string meetingPlaceType { set; get; }
        public int meetingPlaceCapacity { set; get; }
    }
}