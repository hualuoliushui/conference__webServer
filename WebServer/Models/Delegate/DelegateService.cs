using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;

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
        public Status getAll(int meetingID, out List<Delegate> delegates){
            delegates = new List<Delegate>();
            
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
                //获取参会人员信息
                DeviceVO deviceVo = deviceDao.getOne<DeviceVO>(delegateVo.deviceID);
                //获取用户信息
                PersonVO personVo = personDao.getOne<PersonVO>(delegateVo.personID);
                if (deviceVo == null || personVo == null)
                {
                    return Status.FAILURE;
                }

                delegates.Add(
                    new Delegate
                    {
                        userID = personVo.personID,
                        meetingID = meetingID,
                        userName = personVo.personName,
                        userDepartment = personVo.personDepartment,
                        userMeetingRole = delegateVo.personMeetingRole,
                        deviceIndex = deviceVo.deviceIndex
                    });
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 更新参会人员信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="updateDelegate"></param>
        /// <returns></returns>
        public Status update(string userName, UpdateDelegate updateDelegate)
        {
            //验证当前用户的更新当前会议权限
            if (!validateMeeting(userName, updateDelegate.meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            //更新参会人员信息
            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();

            Dictionary<string, object> setlist = new Dictionary<string, object>();

            setlist.Add("deviceID", updateDelegate.deviceID);
            setlist.Add("personID", updateDelegate.userID);

            if(delegateDao.update(setlist,updateDelegate.delegateID)!=1){
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 创建参会人员
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="createDelegate"></param>
        /// <returns></returns>
        public Status create(string userName, CreateDelegate createDelegate)
        {
            //验证当前用户的更新当前会议权限
            if (!validateMeeting(userName, createDelegate.meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            //获取会议状态
            int meetingStatus = getMeetingStatus(createDelegate.meetingID);
            //判断会议是否开启，如果开启，更新“会议更新状态”
            if (IsOpening_Meeting(meetingStatus))
            {
                updateMeetingUpdateStatus(createDelegate.meetingID);
            }
            else if (IsOpended_Meeting(meetingStatus))//如果会议已结束，直接退出
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
                    isSignIn = false
                }) != 1)
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 同时创建多个参会人员
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="createDelegates"></param>
        /// <returns></returns>
        public Status createMultiple(string userName, List<CreateDelegate> createDelegates)
        {
            if (createDelegates == null || createDelegates.Count == 0)
            {
                return Status.ARGUMENT_ERROR;
            }

            //验证当前用户的更新当前会议权限
            if (!validateMeeting(userName, createDelegates[0].meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            //获取会议状态
            int meetingStatus = getMeetingStatus(createDelegates[0].meetingID);
            //判断会议是否开启，如果开启，更新“会议更新状态”
            if (IsOpening_Meeting(meetingStatus))
            {
                updateMeetingUpdateStatus(createDelegates[0].meetingID);
            }
            else if (IsOpended_Meeting(meetingStatus))//如果会议已结束，直接退出
            {
                return Status.FAILURE;
            }

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            foreach (CreateDelegate createDelegate in createDelegates)
            {
                int delegateID = DelegateDAO.getID();

                if (delegateDao.insert<DelegateVO>(
                  new DelegateVO
                  {
                      personID = createDelegate.userID,
                      deviceID = createDelegate.deviceID,
                      meetingID = createDelegate.meetingID,
                      personMeetingRole = createDelegate.userMeetingRole,
                      isSignIn = false
                  }) != 1) 
                {
                    continue;
                }
            }

            return Status.SUCCESS;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="delegates"></param>
        /// <returns></returns>
        public Status deleteMultipe(string userName, List<int> delegates)
        {
            if (delegates == null || delegates.Count == 0)
            {
                return Status.ARGUMENT_ERROR;
            }

            DelegateDAO delegateDao = Factory.getInstance<DelegateDAO>();
            foreach (int delegateID in delegates)
            {
                DelegateVO delegateVo = delegateDao.getOne<DelegateVO>(delegateID);
                if (validateMeeting(userName, delegateVo.meetingID))//有权限就执行删除
                {
                    //获取会议状态
                    int meetingStatus = getMeetingStatus(delegateVo.meetingID);

                    //判断会议是否开启，如果不是”未开启“，直接退出
                    if (!IsNotOpen_Meeting(meetingStatus))
                    {
                        return Status.FAILURE;
                    }
            
                    if (delegateDao.delete(delegateID) != 1)//删除失败就跳过
                    {
                        continue;
                    }
                }
                else // 无权限，直接退出（已删除的不恢复）
                {
                    return Status.PERMISSION_DENIED;
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