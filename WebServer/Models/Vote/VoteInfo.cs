using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Vote
{
    public class VoteInfo
    {
        public int voteID { set; get; }
        public string voteName { set; get; }
        public string voteDescription { set; get; }
        public int voteType { set; get; }
        public int optionNum { set; get; }
        public List<string> options { set; get; }
        public int agendaID { set; get; }
    }
}