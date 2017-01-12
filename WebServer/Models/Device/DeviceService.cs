using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOVO;
using DAL.DAO;
using DAL.DAOFactory;
using WebServer.Models.Role;
using WebServer.App_Start;

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
        public Status create(CreateDevice device)
        {
            try
            {
                Log.DebugInfo(device.ToString());
                //检查参数
                if (string.IsNullOrWhiteSpace(device.IMEI) || device.deviceIndex < 0)
                {
                    return Status.ARGUMENT_ERROR;
                }
                //创建设备ID
                int deviceID = DeviceDAO.getID();

                DeviceDAO deviceDao = Factory.getInstance<DeviceDAO>();
                //插入数据
                if (deviceDao.insert<DeviceVO>(
                    new DeviceVO
                    {
                        deviceID = deviceID,
                        deviceIndex = device.deviceIndex,
                        IMEI = device.IMEI,
                        deviceState = 0
                    }) < 0)
                {
                    return Status.FAILURE;
                }

                return Status.SUCCESS;
            }
            catch (Exception e)
            {
                Log.ErrorInfo(e.StackTrace);
                return Status.SERVER_EXCEPTION;
            }
           
        }

        /// <summary>
        /// 请求 所有设备的 信息
        /// </summary>
        /// <param name="documents"></param>
        /// <returns></returns>
        public Status getAll(out List<Device> devices)
        {
            devices = new List<Device>();
            try
            {
                DeviceDAO deviceDao = Factory.getInstance<DeviceDAO>();
                List<DeviceVO> deviceVolist = deviceDao.getAll<DeviceVO>();
                foreach (DeviceVO deviceVo in deviceVolist)
                {
                    devices.Add(
                        new Device
                        {
                            deviceID = deviceVo.deviceID,
                            IMEI = deviceVo.IMEI,
                            deviceIndex = deviceVo.deviceIndex,
                            deviceFreezeState = deviceVo.deviceState
                        });
                }

                return Status.SUCCESS;
            }
            catch (Exception e)
            {
                Log.ErrorInfo(e.StackTrace);
                return Status.SERVER_EXCEPTION;
            }
        }
        public Status getAllForDelegate(DateTime start, DateTime end, out List<DeviceForDelegate> list)
        {
            list = new List<DeviceForDelegate>();

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            DeviceDAO deviceDao = Factory.getInstance<DeviceDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            List<MeetingVO> meetingVolist = meetingDao.getAll<MeetingVO>();

            int dx = 1;
            var tempMeetings = meetingVolist
                .Where(
                m => (Math.Abs((m.meetingToStartTime - start).TotalHours) < dx ||
                    Math.Abs((m.meetingToStartTime - end).TotalHours) < dx ||
                    Math.Abs((m.meetingStartedTime - start).TotalHours) < dx ||
                    Math.Abs((m.meetingStartedTime - end).TotalHours) < dx)
                    )
                    .Where(m=>m.meetingStatus==1 || m.meetingStatus == 2);

            wherelist.Clear();
            //只允许未冻结的设备作为参会设备
            wherelist.Add("deviceState", 0);
            var deviceVolist = deviceDao.getAll<DeviceVO>(wherelist);

            if (tempMeetings != null && deviceVolist !=null)
            {
                var meetinglist = tempMeetings.ToList();
                foreach (var meetingvo in meetinglist)
                {
                    wherelist.Clear();
                    wherelist.Add("meetingID", meetingvo.meetingID);
                    var delegateVolist = delegateDao.getAll<DelegateVO>(wherelist);

                    if (delegateVolist != null)
                    {
                        foreach (var delegateVo in delegateVolist)
                        {
                            for (int i = 0; i < deviceVolist.Count; i++)
                            {
                                if (deviceVolist[i].deviceID == delegateVo.deviceID)
                                {
                                    deviceVolist.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (deviceVolist != null)
            {
                foreach (var devicevo in deviceVolist)
                {
                    list.Add(new DeviceForDelegate
                    {
                        deviceID = devicevo.deviceID,
                        deviceIndex = devicevo.deviceIndex
                    });
                }
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新时，请求 指定设备的 信息
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public Status getOneForUpdate(out UpdateDevice device, int deviceID)
        {
            device = new UpdateDevice();

            try
            {
                DeviceDAO deviceDao = Factory.getInstance<DeviceDAO>();
                DeviceVO deviceVo = deviceDao.getOne<DeviceVO>(deviceID);
                if (deviceVo == null)
                {
                    return Status.NONFOUND;
                }

                device.deviceID = deviceVo.deviceID;
                device.IMEI = deviceVo.IMEI;
                device.deviceIndex = deviceVo.deviceIndex;

                return Status.SUCCESS;
            }
            catch (Exception e)
            {
                Log.ErrorInfo(e.StackTrace);
                return Status.SERVER_EXCEPTION;
            }
        }

        /// <summary>
        /// 更新时，提交 更新设备的 信息
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public Status update(UpdateDevice device)
        {
            Log.DebugInfo(device.ToString());
            try
            {
                if (string.IsNullOrWhiteSpace(device.IMEI) || device.deviceIndex < 0)
                {
                    return Status.ARGUMENT_ERROR;
                }

                DeviceDAO deviceDao = Factory.getInstance<DeviceDAO>();

                Dictionary<string, object> setlist = new Dictionary<string, object>();
                setlist.Add("IMEI", device.IMEI);
                setlist.Add("deviceIndex", device.deviceIndex);
                if (deviceDao.update(
                    setlist, device.deviceID) < 0)
                    return Status.FAILURE;

                return Status.SUCCESS;
            }
            catch (Exception e)
            {
                Log.ErrorInfo(e.StackTrace);
                return Status.SERVER_EXCEPTION;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <param name="available"></param>
        /// <returns></returns>
        public Status UpdateDeviceAvailable(int deviceID, int available)
        {
            try
            {
                //数据库操作
                DeviceDAO deviceDao = Factory.getInstance<DeviceDAO>();

                Dictionary<string, object> setlist = new Dictionary<string, object>();

                setlist.Add("deviceState", available);
                if (deviceDao.update(setlist, deviceID) < 0)
                {
                    return Status.FAILURE;
                }

                return Status.SUCCESS;
            }
            catch (Exception e)
            {
                Log.ErrorInfo(e.StackTrace);
                return Status.SERVER_EXCEPTION;
            }
        }
    }
}