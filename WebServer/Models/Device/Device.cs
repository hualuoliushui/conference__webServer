using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Device
{
    public class Device
    {
        public int deviceID { set; get; }
        public string IMEI { set; get; }
        public int deviceIndex { set; get; }
        public int deviceFreezeState { set; get; }
    }
}