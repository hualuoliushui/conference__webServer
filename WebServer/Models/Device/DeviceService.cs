using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOVO;

namespace WebServer.Models.Device
{
    //增删改查服务
    public class DeviceService
    {
        public static int create(Device device)
        {
            return 1;
        }

        public static int getAll(out Devices devices)
        {
            devices = new Devices();
            devices.devices = new List<Device>();

            return 1;
        }

        public static int update(Devices devices)
        {

            return 1;
        }

        public static int delete(OleDevices devices)
        {

            return 1;
        }
    }
}