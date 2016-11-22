
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Device
{
    public class CreateDevice
    {
        public string IMEI { set; get; }
        public int deviceIndex { set; get; }
    }
}