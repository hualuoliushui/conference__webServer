using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;
using WebServer.Models.Meeting;
using WebServer.Models.Device;
using WebServer.App_Start;
using System;

namespace WebServer.Models.Delegate
{
    public class DelegateService : Organizor
    {
        /// <summary>
        /// 获取指定会议的参会人员
        /// </summary>
        /// <param name="meetingID"></param>
        /// <param name="delegates"></param>
        /// <returns></returns>
        public Status getAll(int meetingID, out List<DelegateInfo> delegates){
            delegates = new List<DelegateInfo>();
            
            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();

            DeviceDAO deviceDao = Factory.getInstance<DeviceDAO>();

            PersonDAO personDao = Factory.getInstance<PersonDAO>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            wherelist.Add("meetingID", meetingID);
            List<DelegateVO> delegateVos = delegateDao.getAll<DelegateVO>(wherelist);
            if (delegateVos == null)
            {
                return Status.NONFOUND;
            }
            foreach (DelegateVO delegateVo in delegateVos)
            {
                //获取设备信息
                DeviceVO deviceVo = deviceDao.getOne<DeviceVO>(delegateVo.deviceID);
                //获取用户信息
                PersonVO personVo = personDao.getOne<PersonVO>(delegateVo.personID);

                if (deviceVo == null || personVo == null)
                {
                    return Status.FAILURE;
                }

                delegates.Add(
                    new DelegateInfo
                    {
                        delegateID = delegateVo.delegateID,
                        userDepartment = personVo.personDepartment,
                        meetingID = meetingID,
                        userName = personVo.personName,
                        userJob = personVo.personJob,
                        userMeetingRole = delegateVo.personMeetingRole,
                        deviceID = deviceVo.deviceID,
                        deviceIndex = deviceVo.deviceIndex
                    });
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新参会人员信息
        /// </summary>
        /// <param name="updateDelegate"></param>
        /// <returns></returns>
        public Status update(UpdateDelegate updateDelegate)
        {
            //初始化会议操作
            meeting_initOperator(updateDelegate.meetingID);
            //验证当前用户的更新当前会议权限

            //判断会议是否开启，如果正在开启，直接退出
            if (meeting_isOpening())
            {
                return Status.MEETING_OPENING;
            }
            else if (meeting_isOpended())//如果会议已结束，直接退出
            {
                return Status.FAILURE;
            }

            //更新参会人员信息
            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            DelegateVO delegateVo = delegateDao.getOne<DelegateVO>(updateDelegate.delegateID);
            if (delegateVo == null)
            {
                return Status.NONFOUND;
            }

            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            //检查是否存在主讲的议程，如果存在，直接返回
            wherelist.Clear();
            wherelist.Add("meetingID", delegateVo.meetingID);
            wherelist.Add("personID", delegateVo.personID);
            var tempAgenda = agendaDao.getAll<DelegateVO>(wherelist);
            if (tempAgenda != null && tempAgenda.Count > 0)
            {
                return Status.DELEGATE_USED;
            }

            if(delegateVo.deviceID!=updateDelegate.deviceID)//设备改变
            {
                wherelist.Clear();
                wherelist.Add("meetingID", updateDelegate.meetingID);
                wherelist.Add("deviceID", updateDelegate.deviceID);
                if (!unique<DelegateDAO, DelegateVO>(wherelist))
                {
                    return Status.DEVICE_OCCUPY;
                }
            }
            

            Dictionary<string, object> setlist = new Dictionary<string, object>();

            setlist.Add("deviceID", updateDelegate.deviceID);
            setlist.Add("personMeetingRole", updateDelegate.userMeetingRole);

            if(delegateDao.update(setlist,updateDelegate.delegateID)<0 ){
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 创建参会人员
        /// </summary>
        /// <param name="createDelegate"></param>
        /// <returns></returns>
        public Status create(CreateDelegate createDelegate)
        {
            //初始化会议操作
            meeting_initOperator(createDelegate.meetingID);

            bool isUpdate = false;
            //判断会议是否开启，如果正在开启，更新“参会人员更新状态”，数据设置”更新“状态
            if (meeting_isOpening())
            {
                meeting_updateDelegate();
                isUpdate = true;
            }
            else if (meeting_isOpended())//如果会议已结束，直接退出
            {
                return Status.FAILURE;
            }

            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            //验证同名参会人员是否存在
            wherelist.Clear();
            wherelist.Add("meetingID", createDelegate.meetingID);
            wherelist.Add("deviceID", createDelegate.deviceID);
            wherelist.Add("personID", createDelegate.userID);
            if (!unique<DelegateDAO, DelegateVO>(wherelist))
            {
                return Status.NAME_EXIST;
            }

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();

            int delegateID = DelegateDAO.getID();

            if (delegateDao.insert<DelegateVO>(
                new DelegateVO
                {
                    delegateID = delegateID,
                    personID = createDelegate.userID,
                    deviceID = createDelegate.deviceID,
                    meetingID = createDelegate.meetingID,
                    personMeetingRole = createDelegate.userMeetingRole,
                    isSignIn = false,
                    isUpdate = isUpdate //判断是否属于会议中新加入的信息
                }) < 0)
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 同时创建多个参会人员
        /// </summary>
        /// <returns></returns>
        public Status createMultiple(List<DeviceForDelegate> devices,int meetingID, CreateDelegateForMeeting delegates)
        {
            if (delegates == null)
            {
                return Status.SUCCESS;
            }
            List<int> delegateIDs = new List<int>();

            int tempDelegateID = 0;
            DeviceForDelegate tempDevice = null;
            DelegateVO tempDelegate = null;
          
            Status status = Status.SUCCESS;

            int delegateNum = 1;
            if (delegates.speakerIDs != null)
            {
                if (delegates.speakerIDs.Contains(delegates.hostID))
                {
                    delegates.speakerIDs.Remove(delegates.hostID);
                }
                delegateNum += delegates.speakerIDs.Count;
            }
            if (delegates.otherIDs != null)
            {
                if (delegates.otherIDs.Contains(delegates.hostID))
                {
                    delegates.otherIDs.Remove(delegates.hostID);
                }
                delegateNum += delegates.otherIDs.Count;
            }

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            do
            {
                //如果设备数量不足则失败
                if (devices == null || devices.Count < delegateNum)
                {
                    status = Status.DEVICE_LACK;
                    break;
                }

                //添加主持人
                tempDelegateID = DelegateDAO.getID();
                tempDevice = devices[0];
                devices.Remove(tempDevice);

                //验证同名参会人员是否存在
                wherelist.Clear();
                wherelist.Add("meetingID", meetingID);
                wherelist.Add("personID",delegates.hostID );
                if (!unique<DelegateDAO, DelegateVO>(wherelist))
                {
                    return Status.NAME_EXIST;
                }

                tempDelegate = new DelegateVO
                {
                    delegateID = tempDelegateID,
                    deviceID = tempDevice.deviceID,
                    meetingID = meetingID,
                    personID = delegates.hostID,
                    isSignIn = false,
                    isUpdate = false,
                    personMeetingRole = 1
                };
                if (delegateDao.insert<DelegateVO>(tempDelegate) < 0)
                {
                    status = Status.DATABASE_OPERATOR_ERROR;
                    break;
                }
                else
                {
                    delegateIDs.Add(tempDelegateID);
                }

                //添加主讲人
                if (delegates.speakerIDs != null)
                {
                    foreach (var speakerID in delegates.speakerIDs)
                    {
                        tempDelegateID = DelegateDAO.getID();
                        tempDevice = devices[0];
                        devices.Remove(tempDevice);

                        //验证同名参会人员是否存在
                        wherelist.Clear();
                        wherelist.Add("meetingID", meetingID);
                        wherelist.Add("personID", speakerID);
                        if (!unique<DelegateDAO, DelegateVO>(wherelist))
                        {
                            continue;
                        }

                        tempDelegate = new DelegateVO
                        {
                            delegateID = tempDelegateID,
                            deviceID = tempDevice.deviceID,
                            meetingID = meetingID,
                            personID = speakerID,
                            isSignIn = false,
                            isUpdate = false,
                            personMeetingRole = 2
                        };
                        if (delegateDao.insert<DelegateVO>(tempDelegate) < 0)
                        {
                            status = Status.DATABASE_OPERATOR_ERROR;
                            break;
                        }
                        else
                        {
                            delegateIDs.Add(tempDelegateID);
                        }
                    }
                }

                //添加参会人员
                if (delegates.otherIDs != null)
                {
                    foreach (var otherID in delegates.otherIDs)
                    {
                        tempDelegateID = DelegateDAO.getID();
                        tempDevice = devices[0];
                        devices.Remove(tempDevice);

                        //验证同名参会人员是否存在
                        wherelist.Clear();
                        wherelist.Add("meetingID", meetingID);
                        wherelist.Add("personID", otherID);
                        if (!unique<DelegateDAO, DelegateVO>(wherelist))
                        {
                            continue;
                        }

                        tempDelegate = new DelegateVO
                        {
                            delegateID = tempDelegateID,
                            deviceID = tempDevice.deviceID,
                            meetingID = meetingID,
                            personID = otherID,
                            isSignIn = false,
                            isUpdate = false,
                            personMeetingRole = 0
                        };
                        if (delegateDao.insert<DelegateVO>(tempDelegate) < 0)
                        {
                            status = Status.DATABASE_OPERATOR_ERROR;
                            break;
                        }
                        else
                        {
                            delegateIDs.Add(tempDelegateID);
                        }
                    }
                }

            } while (false);

            if (status != Status.SUCCESS)
            {
                //删除之前添加的参会人员
                foreach (var delegateID in delegateIDs)
                {
                    delegateDao.delete(delegateID);
                }
            }

            return status;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="delegates"></param>
        /// <returns></returns>
        public Status deleteMultipe(List<int> delegates)
        {
            if (delegates == null || delegates.Count == 0)
            {
                return Status.SUCCESS;
            }

            //用于出错后恢复数据
            var backup = new List<DelegateVO>();
            Status status = Status.SUCCESS;

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            foreach (int delegateID in delegates)
            {
                DelegateVO delegateVo = delegateDao.getOne<DelegateVO>(delegateID);
                if (delegateVo == null)
                {
                    status = Status.NONFOUND;
                }

                //初始化会议操作
                meeting_initOperator(delegateVo.meetingID);
                //判断会议是否开启，如果不是”未开启“，直接退出
                if (!meeting_isNotOpen())
                {
                    status = Status.MEETING_OPENING;
                    break;
                }

                Dictionary<string, object> wherelist = new Dictionary<string, object>();
                //检查是否为主讲人，且是否存在主讲的议程，如果存在，直接返回
                wherelist.Clear();
                wherelist.Add("meetingID", delegateVo.meetingID);
                wherelist.Add("personID", delegateVo.personID);
                var tempAgenda = agendaDao.getAll<AgendaVO>(wherelist);
                if (tempAgenda != null && tempAgenda.Count > 0)
                {
                    status = Status.DELEGATE_USED;
                    break;
                }

                backup.Add(delegateVo);
                if (delegateDao.delete(delegateID) < 0)//删除失败就 恢复数据，返回
                {
                    status = Status.FAILURE;
                    break;
                }      
            }
            if (status != Status.SUCCESS)
            {
                foreach (var delegateVo in backup)
                {
                    delegateDao.insert<DelegateVO>(delegateVo);
                }
            }
            return status;
        }


        public Status getSpeakerForAgenda(int meetingID, out List<SpeakerForAgenda> speakers)
        {
            speakers = new List<SpeakerForAgenda>();
            //获取会议中的全部主讲人
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            wherelist.Add("meetingID", meetingID);
            wherelist.Add("personMeetingRole", 2);//主讲人

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            List<DelegateVO> delegateVolist = delegateDao.getAll<DelegateVO>(wherelist);
            if (delegateVolist == null)
            {
                return Status.NONFOUND;
            }

            PersonDAO personDao = Factory.getInstance<PersonDAO>();

            //将信息插入返回值
            foreach (DelegateVO delegateVo in delegateVolist)
            {
                //获取主讲人信息
                PersonVO personVo = personDao.getOne<PersonVO>(delegateVo.personID);
                if (personVo == null)
                    continue;

                speakers.Add(
                    new SpeakerForAgenda
                    {
                        userID = personVo.personID,
                        userName = personVo.personName
                    });
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 删除会议时使用
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public Status deleteAll(int meetingID)
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            wherelist.Add("meetingID", meetingID);
            delegateDao.delete(wherelist);
            return Status.SUCCESS;
        }

        public int getSeatType(int meetingID,out int meetingPlaceID)
        {
            meetingPlaceID = -1;
            try
            {
                var wherelist = new Dictionary<string, object>();

                //查询会场布局信息
                var meetingDao = Factory.getInstance<MeetingDAO>();
                var meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();
                var longTableDao = Factory.getInstance<LongTableDAO>();

                var meetingVo = meetingDao.getOne<MeetingVO>(meetingID);
                if (meetingVo == null)
                {
                    throw new Exception("会议不存在");
                }

                wherelist.Clear();
                wherelist.Add("meetingPlaceID", meetingVo.meetingPlaceID);

                var meetingPlaceVo = meetingPlaceDao.getOne<MeetingPlaceVO>(wherelist);

                if (meetingPlaceVo == null)
                {
                    throw new Exception("会场不存在");
                }
                meetingPlaceID = meetingPlaceVo.meetingPlaceID;

                return meetingPlaceVo.seatType;
            }
            catch (Exception e)
            {
                Log.LogInfo("getSeatType(" + meetingID + "):", e);
                return -1;
            }

        }

        public SeatArrange_LongTableModel getSeatInfos_LongTable(int meetingID,int meetingPlaceID)
        {
            SeatArrange_LongTableModel model = new SeatArrange_LongTableModel();

            try
            {
                do
                {
                    var wherelist = new Dictionary<string, object>();

                    //查询会场布局信息
                    var meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();
                    var longTableDao = Factory.getInstance<LongTableDAO>();

                    wherelist.Clear();
                    wherelist.Add("meetingPlaceID", meetingPlaceID);

                    var longTableVo = longTableDao.getOne<LongTableVO>(wherelist);

                    if (longTableVo == null)
                    {
                        throw new Exception("长桌会场类型参数不存在");
                    }

                    model.upNum = longTableVo.upNum;
                    model.downNum = longTableVo.downNum;
                    model.leftNum = longTableVo.leftNum;
                    model.rightNum = longTableVo.rightNum;

                    //获取现有座位信息
                    model.seatInfos = getSeatInfos(meetingID);

                } while (false);

                return model;
            }
            catch (System.Exception e)
            {
                Log.LogInfo("getSeatInfos(" + meetingID + "):", e);
                model.seatInfos = new List<SeatInfo>();
                return model;//返回空列表
            }
        }

        public Status updateSeatInfos(List<SeatInfo> seatInfos)
        {
            try
            {
                do
                {
                    if (seatInfos == null || seatInfos.Count == 0)
                        break;//跳出执行

                    var delegateDao = Factory.getInstance<DelegateDAO>();

                    var setlist = new Dictionary<string, object>();
                    foreach (var seatInfo in seatInfos)
                    {
                        setlist.Clear();
                        setlist.Add("seatIndex", seatInfo.seatIndex);

                        delegateDao.update(setlist, seatInfo.delegateID);
                    }

                } while (false);
            }
            catch (System.Exception e)
            {
                Log.LogInfo("updateSeatInfos():", e);
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }


        ///////////=========================================================
        private List<SeatInfo> getSeatInfos(int meetingID)
        {
            try
            {
                var wherelist = new Dictionary<string, object>();

                //查询参会人员信息
                var seatInfos = new List<SeatInfo>();

                wherelist.Clear();
                wherelist.Add("meetingID", meetingID);

                var delegateDao = Factory.getInstance<DelegateDAO>();
                var deviceDao = Factory.getInstance<DeviceDAO>();
                var personDao = Factory.getInstance<PersonDAO>();

                var delegateVos = delegateDao.getAll<DelegateVO>(wherelist);
                if (delegateVos != null)
                {
                    foreach (var delegateVo in delegateVos)
                    {
                        var seatInfo = new SeatInfo();

                        seatInfo.delegateID = delegateVo.delegateID;
                        seatInfo.seatIndex = delegateVo.seatIndex;

                        var deviceVo = deviceDao.getOne<DeviceVO>(delegateVo.deviceID);
                        if (deviceVo == null)
                            continue;
                        seatInfo.deviceIndex = deviceVo.deviceIndex;

                        var personVo = personDao.getOne<PersonVO>(delegateVo.personID);
                        if (personVo == null)
                            continue;
                        seatInfo.userName = personVo.personName;
                        seatInfo.userLevel = personVo.personLevel;

                        //添加到座位安排列表
                        seatInfos.Add(seatInfo);
                    }
                }
                return seatInfos;
            }
            catch (System.Exception e)
            {
                Log.LogInfo("getSeatInfos(" + meetingID + "):", e);
                return new List<SeatInfo>();//返回空列表
            }
        }
    }
}