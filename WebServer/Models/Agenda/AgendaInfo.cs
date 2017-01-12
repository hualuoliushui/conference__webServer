using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Agenda
{
    public class AgendaInfo
    {
        public int agendaID { set; get; }
        public string agendaName { set; get; }
        public int agendaDuration { set; get; }
        public string userName { set; get; }
        public int meetingID { set; get; }

        public override String ToString()
        {
            return "{议程:议程ID:" + agendaID +
                ",议程名称:" + agendaName +
                ",议程时长:" + agendaDuration + "分钟" +
                ",主讲人:" + userName +
                ",所属会议ID:" + meetingID + "}";
        }
    }
}