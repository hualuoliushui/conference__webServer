using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.Meeting
{
    //展示会议内容项
    public class ShowMeetingItemModel
    {
        public MeetingInfo meeting { set; get; }

        public List<Agenda.AgendaInfo> agendas { set; get; }

        public List<Delegate.DelegateInfo> delegates { set; get; }

        public List<MeetingPlace.MeetingPlaceForMeeting> meetingPlaces { set; get; }
    }

    //展示会议信息
    public class ShowMeetingModel
    {
        public MeetingInfo meeting { set; get; }
        public List<MeetingPlace.MeetingPlaceForMeeting> meetingPlaces { set; get; }
        public List<User.UserForDelegate> users { set; get; }
    }

     public class MeetingInfo
    {
        [Required(ErrorMessage = "会议ID不为空")]
        public int meetingID { set; get; }

        [Required(ErrorMessage="会议名称不为空")]
        [StringLength(100,MinimumLength=2,ErrorMessage="会议名称2-100个字符")]
        public string meetingName { set; get; }

        [Required(ErrorMessage="会场未选择")]
        public int meetingPlaceID { set; get; }

        [Required(ErrorMessage=("会议概述不为空"))]
        public string meetingSummary { set; get; }
        
        [Required(ErrorMessage="会议计划开始时间不为空")]
        public DateTime meetingToStartTime { set; get; }

        [Required(ErrorMessage="会议计划结束时间不为空")]
        public DateTime meetingStartedTime { set; get; }

        public int meetingStatus { set; get; }
    }
}