using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.Models.Device;

namespace WebServer.Models.Delegate
{
    public class DelegateInfo
    {
        public int delegateID { set; get; }
        public int meetingID { set; get; }
        public string userName { set; get; }
        public string userDepartment { set; get; }
        public string userJob { set; get; }
        //会议中参会人员的角色 ：0：参会人员 ,1:主持人 ，2:主讲人
        public int userMeetingRole { set; get; }
        //设备标识
        public int deviceID { set; get; }
        //设备编号
        public int deviceIndex { set; get; }
    }

    public class ShowDelegateModel
    {
        public List<DelegateInfo> delegates { set; get; }
        public List<DeviceForDelegate> devices { set; get; }
    }
}