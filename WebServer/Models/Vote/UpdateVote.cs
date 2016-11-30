using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Vote
{
    public class UpdateVote
    {
        public int voteID { set; get; }

        public int agendaID { set; get; }

        public int voteIndex { set; get; }

        public string voteName { set; get; }

        public string voteDescription { get; set; }

        public int voteType { set; get; }

        public int optionNum { set; get; }

        public List<string> voteOptions { set; get; }
    }
}