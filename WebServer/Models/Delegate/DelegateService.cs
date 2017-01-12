using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;
using WebServer.Models.Meeting;
using WebServer.Models.Device;

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

            int delegateNum = 1 +
                (delegates.speakerIDs != null ? delegates.speakerIDs.Count : 0) +
                (delegates.otherIDs != null ? delegates.otherIDs.Count : 0);

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            do
            {
                //如果设备数量不足则失败
                if (devices == null || devices.Count < delegateNum)
                {
                    status = Status.FAILURE;
                    break;
                }

                //添加主持人
                tempDelegateID = DelegateDAO.getID();
                tempDevice = devices[0];
                devices.Remove(tempDevice);
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
                if (delegates.speakerIDs != null)
                {
                    foreach (var otherID in delegates.otherIDs)
                    {
                        tempDelegateID = DelegateDAO.getID();
                        tempDevice = devices[0];
                        devices.Remove(tempDevice);
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
                return Status.ARGUMENT_ERROR;
            }

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            foreach (int delegateID in delegates)
            {
                DelegateVO delegateVo = delegateDao.getOne<DelegateVO>(delegateID);
                if (delegateVo == null)
                {
                    return Status.NONFOUND;
                }
                //初始化会议操作
                meeting_initOperator(delegateVo.meetingID);
                    //判断会议是否开启，如果不是”未开启“，直接退出
                    if (!meeting_isNotOpen())
                    {
                        return Status.MEETING_OPENING;
                    }
            
                    if (delegateDao.delete(delegateID) < 0)//删除失败就跳过
                    {
                        continue;
                    }   
            }
            return Status.SUCCESS;
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
        
    }
}