using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Models.Agenda
{
    public class CreateAgenda
    {
        public string agendaName { set; get; }
        public int agendaDuration { set; get; }
        public int userID { set; get; }
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
