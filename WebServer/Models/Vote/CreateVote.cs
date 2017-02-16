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

        [Required(ErrorMessage = "投票描述不为空")]
        [StringLength(1000, MinimumLength = 0, ErrorMessage = "投票描述1-1000个字符")]
        public string voteDescription { get; set; }

        [Required(ErrorMessage = "投票类型未选择")]
        [Range(1, int.MaxValue, ErrorMessage = "投票类型{0}：可选数量在{1}到{2}之间.")]
        public int voteType { set; get; }

        [Required(ErrorMessage="投票选项不为空")]
        public List<string> voteOptions { set; get; }
    }
}