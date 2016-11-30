using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Device
{
    public class DeviceForDelegate
    {
        public int deviceID { set; get; }

        public int deviceIndex { set; get; }

        public override String ToString()
        {
            return "{设备:" +
               "设备ID:" + deviceID +
               ",设备编号:" + deviceIndex +
               "}";
        }
    }
}