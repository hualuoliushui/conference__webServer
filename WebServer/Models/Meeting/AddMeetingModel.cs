using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Models.Delegate;

namespace WebServer.Models.Meeting
{
    public class AddMeetingModel
    {
        public MeetingInfo meeting { set; get; }

        public CreateDelegateForMeeting delegates { set; get; }
    }
}