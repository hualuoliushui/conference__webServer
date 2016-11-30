using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Vote
{
    public class CreateVote
    {
        public int agendaID { set; get; }

        public string voteName { set; get; }

        public string voteDescription { get; set; }

        public int voteType { set; get; }

        public int optionNum { set; get; }

        public List<string> voteOptions { set; get; }
    }
}