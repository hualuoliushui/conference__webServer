using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace WebServer.Models.Agenda
{
    public class CreateAgenda
    {
        [Required(ErrorMessage = "议程名称不为空")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "议程名称2-100个字符")]
        public string agendaName { set; get; }

        [Required(ErrorMessage = "议程时长不为空")]
        [Range(1, 10000, ErrorMessage = "议程时长必须在{1}和{2}之间")]
        public int agendaDuration { set; get; }

        [Required(ErrorMessage = "主讲人ID不为空")]
        public int userID { set; get; }

        [Required(ErrorMessage = "会议名称不为空")]
        public int meetingID { set; get; }

        public override String ToString()
        {
            return "{议程:" +
                "议程名称:" + agendaName +
                ",议程时长:" + agendaDuration + "分钟" +
                ",主讲人ID:" + userID +
                ",所属会议ID:" + meetingID + "}";
        }
    }
}
