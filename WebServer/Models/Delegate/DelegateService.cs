using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;

namespace WebServer.Models.Delegate
{
    public class DelegateService
    {
        private static bool validate(string userName,int meetingID){
             //验证当前用户的更新当前会议权限
            MeetingDAO meetingDao = Factory.getMeetingDAOInstance();
            MeetingVO meetingVo = meetingDao.getMeetingByMeetingID(meetingID);
            if (meetingVo == null)
            {
                return false;
            }

            UserDAO userDao = Factory.getUserDAOInstance();

            UserVO userVo = userDao.getUserByUserID(meetingVo.userID);
            if (userVo == null)
            {
                return false;
            }

            return (string.Compare(userVo.userName, userName) == 0);
        }
        public static Status getAll(int meetingID, out List<Delegate> delegates){
            delegates = new List<Delegate>();

            DelegateDAO delegateDao = Factory.getDelegateDAOInstance();

            DeviceDAO deviceDao = Factory.getDeviceDAOInstance();

            UserDAO userDao = Factory.getUserDAOInstance();

            List<DelegateVO> delegateVos = delegateDao.getDelegateListByMeetingID(meetingID);
            if (delegateVos.Count == 0)
            {
                return Status.NONFOUND;
            }
            foreach (DelegateVO delegateVo in delegateVos)
            {
                DeviceVO deviceVo = deviceDao.getDeviceByDeviceID(delegateVo.deviceID);
                UserVO userVo = userDao.getUserByUserID(delegateVo.userID);

                delegates.Add(
                    new Delegate
                    {
                        userID = userVo.userID,
                        meetingID = meetingID,
                        userName = userVo.userName,
                        userDepartment = userVo.userDepartment,
                        userMeetingRole = delegateVo.userMeetingRole,
                        deviceIndex = deviceVo.deviceIndex
                    });
            }

            return Status.SUCCESS;
        }

        public static Status update(string userName, UpdateDelegate updateDelegate)
        {
            //验证当前用户的更新当前会议权限
            if(!validate(userName,updateDelegate.meetingID)){
                return Status.PERMISSION_DENIED;
            }

            //更新参会人员信息
            DelegateDAO delegateDao = Factory.getDelegateDAOInstance();
            DelegateVO delegateVo = delegateDao.
                getDelegateByMeetingIDAndUserID(
                updateDelegate.meetingID,
                updateDelegate.userID);
            if(delegateVo == null){
                return Status.FAILURE;
            }

            if(!delegateDao.updateDelegate(
                new DelegateVO
                {
                    deviceID = updateDelegate.deviceID,
                    meetingID = delegateVo.meetingID,
                    userID = delegateVo.userID,
                    userMeetingRole = updateDelegate.userMeetingRole,
                    isSignIn = delegateVo.isSignIn
                })){
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        public static Status create(string userName, CreateDelegate createDelegate)
        {
            //验证当前用户的更新当前会议权限
            if (!validate(userName, createDelegate.meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            DelegateDAO delegateDao = Factory.getDelegateDAOInstance();
            if (!delegateDao.addDelegate(
                new DelegateVO
                {
                    userID = createDelegate.userID,
                    deviceID = createDelegate.deviceID,
                    meetingID = createDelegate.meetingID,
                    userMeetingRole = createDelegate.userMeetingRole,
                    isSignIn = false
                }))
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        public static Status createMultiple(string userName, List<CreateDelegate> createDelegates)
        {
            if (createDelegates == null || createDelegates.Count == 0)
            {
                return Status.ARGUMENT_ERROR;
            }

            //验证当前用户的更新当前会议权限
            if (!validate(userName, createDelegates[0].meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            DelegateDAO delegateDao = Factory.getDelegateDAOInstance();
            foreach (CreateDelegate createDelegate in createDelegates)
            {
                if (!delegateDao.addDelegate(
                  new DelegateVO
                  {
                      userID = createDelegate.userID,
                      deviceID = createDelegate.deviceID,
                      meetingID = createDelegate.meetingID,
                      userMeetingRole = createDelegate.userMeetingRole,
                      isSignIn = false
                  })) 
                {
                    continue;
                }
            }

            return Status.SUCCESS;
        }

        public static Status deleteMultipe(string userName, List<DeleteDelegate> delegates)
        {
            if (delegates == null || delegates.Count == 0)
            {
                return Status.ARGUMENT_ERROR;
            }

            //验证当前用户的更新当前会议权限
            if (!validate(userName, delegates[0].meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            DelegateDAO delegateDao = Factory.getDelegateDAOInstance();
            foreach (DeleteDelegate deleteDelegate in delegates)
            {
                if (!delegateDao.deleteDelegateByMeetingIDAndUserID(deleteDelegate.meetingID, deleteDelegate.userID))
                {
                    continue;
                }
            }

            return Status.SUCCESS;
        }
    }
}