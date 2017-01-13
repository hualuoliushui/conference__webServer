using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.Vote
{
    public class UpdateVote
    {
        [Required(ErrorMessage = "投票ID不为空")]
        public int voteID { set; get; }

        [Required(ErrorMessage = "投票名称不为空")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "投票名称2-100个字符")]
        public string voteName { set; get; }

        [StringLength(1000, MinimumLength = 0, ErrorMessage = "投票描述0-1000个字符")]
        public string voteDescription { get; set; }

        [Required(ErrorMessage = "投票类型未选择")]
        [Range(0, 2, ErrorMessage = "投票类型必须在{1}和{2}之间")]
        public int voteType { set; get; }

        public List<string> voteOptions { set; get; }
    }
}