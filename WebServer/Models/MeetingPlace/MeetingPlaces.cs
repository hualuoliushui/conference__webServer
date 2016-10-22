using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOVO;

namespace WebServer.Models.MeetingPlace
{
    //提供给浏览器的会场信息列表类
    public class MeetingPlaces
    {
        public List<MeetingPlace> meetingPlaces;
    }
}