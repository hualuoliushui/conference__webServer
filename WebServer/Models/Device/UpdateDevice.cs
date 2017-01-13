using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebServer.Models.Device
{
    public class UpdateDevice
    {
        [Required(ErrorMessage = "设备ID不为空")]
        public int deviceID { set; get; }

        [Required(ErrorMessage = "设备IMEI不为空")]
        [StringLength(15, MinimumLength = 15, ErrorMessage = "设备IMEI长度不为15")]
        public string IMEI { set; get; }

        [Required(ErrorMessage = "设备索引不为空")]
        [Range(1, int.MaxValue, ErrorMessage = "设备索引必须在{1}和{2}之间")]
        public int deviceIndex { set; get; }

        public override String ToString()
        {
            return "{修改设备:" +
               "设备ID:" + deviceID +
               ",设备IMEI:" + IMEI +
               ",设备编号:" + deviceIndex +
               "}";
        }
    }
}