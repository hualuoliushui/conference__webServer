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
        [Required(ErrorMessage = "参会人员ID不为空")]
        public int userID { set; get; }

        [Required(ErrorMessage = "会议ID不为空")]
        public int meetingID { set; get; }

        //会议中参会人员的角色 ：0：参会人员 ,1:主持人 ，2:主讲人
        [Required(ErrorMessage = "参会人员角色不为空")]
        [Range(0, 2, ErrorMessage = "参会人员角色必须在{1}和{2}之间")]
        public int userMeetingRole { set; get; }

        //设备标识
        [Required(ErrorMessage = "设备ID不为空")]
        public int deviceID { set; get; }
    }
}