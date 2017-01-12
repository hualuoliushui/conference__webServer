using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.Delegate
{
    public class CreateDelegateForMeeting
    {
        [Required(ErrorMessage="主持人不为空")]
        public int hostID { set; get; }

        public List<int> speakerIDs { set; get; }

        public List<int> otherIDs { set; get; }
    }

    public class CreateDelegate
    {
        public int userID { set; get; }
        public int meetingID { set; get; }
        public int deviceID { set; get; }
        public int userMeetingRole { set; get; }
    }
}