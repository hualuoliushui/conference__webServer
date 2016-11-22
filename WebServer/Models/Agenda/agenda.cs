using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Agenda
{
    public class Agenda
    {
        public int agendaID { set; get; }
        public string agendaName { set; get; }
        public int agendaDuration { set; get; }
        public string userName { set; get; }
        public int meetingID { set; get; }
    }
}