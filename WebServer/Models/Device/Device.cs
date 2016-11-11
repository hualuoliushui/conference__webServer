using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Device
{
    public class Device
    {
        public string deviceID { set; get; }
        public int deviceIndex { set; get; }
        public int deviceAvailable { set; get; }
    }
}