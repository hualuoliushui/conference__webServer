using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOVO;
using DAL.DAO;
using DAL.DAOFactory;
using WebServer.Models.Role;

namespace WebServer.Models.Device
{
    /// <summary>
    /// 服务操作类
    /// 处理有关设备管理的请求
    /// </summary>
    public class DeviceService
    {
        /// <summary>
        /// 创建设备
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Status create(Device device)
        {
            if (string.IsNullOrWhiteSpace(device.deviceID) || device.deviceIndex < 0)
            {
                return Status.ARGUMENT_ERROR;
            }

            DeviceDAO deviceDao = Factory.getDeviceDAOInstance();
            if (!deviceDao.addDevice(
                new DeviceVO {
                    deviceID = device.deviceID, 
                    deviceIndex = device.deviceIndex, 
                    deviceAvailable = 0 
                }))
            {
                return Status.FAILURE;
            }

                return Status.SUCCESS;
        }

        /// <summary>
        /// 请求 所有设备的 信息
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public static Status getAll(out List<Device> devices)
        {
            devices = new List<Device>();

            DeviceDAO deviceDao = Factory.getDeviceDAOInstance();
            List<DeviceVO> deviceVOs = deviceDao.getDeviceList();
            foreach (DeviceVO vo in deviceVOs)
            {
                devices.Add(
                    new Device { 
                        deviceID = vo.deviceID, 
                        deviceIndex = vo.deviceIndex,
                        deviceAvailable = vo.deviceAvailable 
                    });
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新时，请求 指定设备的 信息
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public static Status getOne(out UpdateDevice device, string deviceID)
        {
            device = new UpdateDevice();

            DeviceDAO deviceDao = Factory.getDeviceDAOInstance();
            DeviceVO vo = deviceDao.getDeviceByDeviceID(deviceID);
            if (vo == null)
            {
                return Status.NONFOUND;
            }

            device.deviceID = vo.deviceID;
            device.deviceIndex = vo.deviceIndex;

            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新时，提交 更新设备的 信息
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Status update(UpdateDevice device)
        {
            if (string.IsNullOrWhiteSpace(device.deviceID) || device.deviceIndex < 0 )
            {
                return Status.ARGUMENT_ERROR;
            }

            DeviceDAO deviceDao = Factory.getDeviceDAOInstance();
            if (deviceDao.updateDevice(
                new DeviceVO { 
                    deviceID = device.deviceID,
                    deviceIndex = device.deviceIndex, 
                })) ;
                return Status.SUCCESS;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        public static Status UpdateDeviceAvailable(string deviceID, int available)
        {
            //检查参数
            if (string.IsNullOrWhiteSpace(deviceID) 
                || (available != 0 && available != 1))
            {
                return Status.ARGUMENT_ERROR;
            }

            //修正字符串
            deviceID = deviceID.Trim();

            //数据库操作
            DeviceDAO deviceDao = Factory.getDeviceDAOInstance();
            if (!deviceDao.updateDeviceAvailable(deviceID,available))
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }
    }
}