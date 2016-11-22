using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Delegate
{
    public class CreateDelegate
    {
        public int userID { set; get; }
        public int meetingID { set; get; }
        public int deviceID { set; get; }
        public int userMeetingRole { set; get; }
    }
}