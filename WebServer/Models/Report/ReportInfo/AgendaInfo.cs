using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Report.ReportInfo
{
    public class AgendaInfo
    {
        public int agendaID { set; get; }
        public string agendaName { set; get; }
        public string agendaIndex { set; get; }
        public string agendaSpeakerName { set; get; }
        public string agendaDuration { set; get; }

        public List<ReportInfo.FileInfo> fileInfos { set; get; }
        public List<ReportInfo.VoteInfo> voteInfos { set; get; }
    }
}