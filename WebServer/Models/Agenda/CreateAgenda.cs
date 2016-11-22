using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Models.Agenda
{
    class CreateAgenda
    {
        public string agendaName { set; get; }
        public int agendaDuration { set; get; }
        public int userID { set; get; }
        public int meetingID { set; get; }
    }
}
