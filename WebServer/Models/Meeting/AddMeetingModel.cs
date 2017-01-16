using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using WebServer.Models.Delegate;

namespace WebServer.Models.Meeting
{
    public class AddMeetingModel
    {
        [Required(ErrorMessage="会议数据丢失")]
        public MeetingInfo meeting { set; get; }
        [Required(ErrorMessage="参会人员数据丢失")]
        public CreateDelegateForMeeting delegates { set; get; }
    }
}