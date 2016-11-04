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
            //判断操作成功与否,返回相应状态码
            ///////


            return 1;
        }

        public static int getOne(out Device device, string deviceID)
        {
            device = new Device();

            return 1;
        }

        public static int update(Devices devices)
        {

            return 1;
        }

        public static int delete(OldDevices devices)
        {

            return 1;
        }
    }
}