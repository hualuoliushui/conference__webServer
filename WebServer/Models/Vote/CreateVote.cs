using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.Vote
{
    public class CreateVote
    {
        [Required(ErrorMessage = "议程ID不为空")]
        public int agendaID { set; get; }

        [Required(ErrorMessage = "投票名称不为空")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "投票名称2-100个字符")]
        public string voteName { set; get; }

        [StringLength(1000, MinimumLength = 0, ErrorMessage = "投票描述0-1000个字符")]
        public string voteDescription { get; set; }

        [Required(ErrorMessage = "投票类型未选择")]
        [Range(0, 2, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int voteType { set; get; }

        public List<string> voteOptions { set; get; }
    }
}