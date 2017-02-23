using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Report.ReportInfo
{
    public class VoteInfo
    {
        public string voteName { set; get; }
        public string voteDescription { set; get; }
        public int voteType { set; get; }
        public int optionNum { set; get; }
        public int voteStatus { set; get; }
        public List<OptionInfo> optionInfos { set; get; }
    }

    public class OptionInfo
    {
        public string optionName { set; get; }
        public int optionResult { set; get; }
    }
}