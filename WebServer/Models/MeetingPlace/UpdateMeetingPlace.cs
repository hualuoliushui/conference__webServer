using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.MeetingPlace
{
    //提供给浏览器的删除会场的标识列表类
    public class UpdateMeetingPlace
    {
        [Required(ErrorMessage = "会场ID不为空")]
        public int meetingPlaceID { set; get; }

        [Required(ErrorMessage = "会场名称不为空")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "会场名称1-100个字符")]
        public string meetingPlaceName { set; get; }

        [Required(ErrorMessage = "会场容量不为空")]
        [Range(1, 10000, ErrorMessage = "会场容量必须在{1}和{2}之间")]
        public int meetingPlaceCapacity { set; get; }


        public override String ToString()
        {
            return "{修改会场：" +
                "会场ID：" + meetingPlaceID +
                ",会场名称：" + meetingPlaceName +
                ",会场容量：" + meetingPlaceCapacity +
                "}";
        }
    }
}