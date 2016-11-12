using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOVO;
using DAL.DAO;
using DAL.DAOFactory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Meeting
{
    public class MeetingService
    {
        private static int MeetingNameMin = 2;
        private static int MeetingNameMax = 16;

        private static int MeetingSummaryMax = 50;

        private static int MeetingIDMax = Factory.getMeetingDAOInstance().getIDMax();

        private static int getMeetingID()
        {
            int id = 0;
            object lockObject = new object();
            lock (lockObject)
            {
                id = ++MeetingIDMax;
            }
            return id;
        }

        //检查参数格式
        private static bool checkFormat(string meetingName, string meetingSummary, DateTime meetingToStartTime)
        {
            return (meetingName.Length >= MeetingNameMin
                && meetingName.Length <= MeetingNameMax
                && meetingSummary.Length <= MeetingSummaryMax
                && (meetingToStartTime - DateTime.Now).TotalSeconds < 0 );  //时间：必须是未来
        }

        public static Status create(string userName, CreateMeeting meeting)
        {
            //修正字符串
            meeting.meetingName = meeting.meetingName.Trim();
            meeting.meetingSummary = meeting.meetingSummary.Trim();
            //检查参数格式
            if (!checkFormat(meeting.meetingName, meeting.meetingSummary, meeting.meetingToStartTime))
            {
                return Status.FORMAT_ERROR;
            }

            UserDAO userDao = Factory.getUserDAOInstance();
            UserVO userVo = userDao.getUserByUserName(userName);

            MeetingDAO meetingDao = Factory.getMeetingDAOInstance();
            int meetingID = getMeetingID();
            if (!meetingDao.addMeeting(
                new MeetingVO
                {
                    meetingID = meetingID,
                    meetingName = meeting.meetingName,
                    meetingPlaceID = meeting.meetingPlaceID,
                    meetingSummary = meeting.meetingSummary,
                    meetingToStartTime = meeting.meetingToStartTime,
                    meetingStatus = 1,//未开
                    meetingDuration = 0,
                    meetingStartedTime = DateTime.Now,
                    meetingUpdateStatus = 0, //无更新
                    userID = userVo.userID
                }))
            {
                return Status.FAILURE;
            }


            return Status.SUCCESS;
        }

        public static Status getAll(out List<TipMeeting> meetings)
        {
            meetings = new List<TipMeeting>();

            MeetingDAO meetingDao = Factory.getMeetingDAOInstance();

            List<MeetingVO> meetingVos = meetingDao.getMeetingList();

            if (meetingVos.Count == 0)
            {
                return Status.NONFOUND;
            }

            foreach (MeetingVO meetingVo in meetingVos)
            {
                meetings.Add(
                    new TipMeeting
                    {
                        meetingID = meetingVo.meetingID,
                        meetingName = meetingVo.meetingName,
                        meetingStatus = meetingVo.meetingStatus
                    });
            }

            return Status.SUCCESS;
        }

        public static Status getOne(int meetingID,out Meeting meeting){
            meeting = new Meeting();

            MeetingDAO meetingDao = Factory.getMeetingDAOInstance();
            MeetingVO meetingVo = meetingDao.getMeetingByMeetingID(meetingID);

            if (meetingVo == null)
            {
                return Status.NONFOUND;
            }

            MeetingPlaceDAO meetingPlaceDao = Factory.getMeetingPlaceDAOInstance();
            MeetingPlaceVO meetingPlaceVo = meetingPlaceDao.
                getMeetingPlaceByMeetingPlaceID(meetingVo.meetingPlaceID);

            meeting.meetingID = meetingVo.meetingID;
            meeting.meetingName = meetingVo.meetingName;
            meeting.meetingPlaceName = meetingPlaceVo.meetingPlaceName;
            meeting.meetingSummary = meetingVo.meetingSummary;
            meeting.meetingToStartTime = meetingVo.meetingToStartTime;
            meeting.meetingStatus = meetingVo.meetingStatus;

            return Status.SUCCESS;
        }

        public static Status getOneForUpdate(string userName,int meetingID, out UpdateMeeting meeting)
        {
            meeting = new UpdateMeeting();

            MeetingDAO meetingDao = Factory.getMeetingDAOInstance();
            MeetingVO meetingVo = meetingDao.getMeetingByMeetingID(meetingID);

            if (meetingVo == null)
            {
                return Status.NONFOUND;
            }

            UserDAO userDao = Factory.getUserDAOInstance();
            UserVO userVo = userDao.getUserByUserID(meetingVo.userID);

            if (string.Compare(userVo.userName, userName) != 0)
            {
                return Status.PERMISSION_DENIED;
            }

            meeting.meetingID = meetingVo.meetingID;
            meeting.meetingName = meetingVo.meetingName;
            meeting.meetingPlaceID = meetingVo.meetingPlaceID;
            meeting.meetingSummary = meetingVo.meetingSummary;
            meeting.meetingToStartTime = meetingVo.meetingToStartTime;

            return Status.SUCCESS;
        }



        public static Status update(string userName, UpdateMeeting meeting)
        {
            //修正字符串
            meeting.meetingName = meeting.meetingName.Trim();
            meeting.meetingSummary = meeting.meetingSummary.Trim();
            //检查参数格式
            if (!checkFormat(meeting.meetingName, meeting.meetingSummary, meeting.meetingToStartTime))
            {
                return Status.FORMAT_ERROR;
            }

            UserDAO userDao = Factory.getUserDAOInstance();
            UserVO userVo = userDao.getUserByUserName(userName);

            MeetingDAO meetingDao = Factory.getMeetingDAOInstance();
            MeetingVO meetingVo = meetingDao.getMeetingByMeetingID(meeting.meetingID);
            if (meetingVo == null)
            {
                return Status.FAILURE;
            }
            if (!meetingDao.updateMeeting(
                new MeetingVO
                {
                    meetingID = meeting.meetingID,
                    meetingName = meeting.meetingName,
                    meetingPlaceID = meeting.meetingPlaceID,
                    meetingSummary = meeting.meetingSummary,
                    meetingToStartTime = meeting.meetingToStartTime,
                    meetingStatus = meetingVo.meetingStatus,//未开
                    meetingDuration = meetingVo.meetingDuration,
                    meetingStartedTime = meetingVo.meetingStartedTime,
                    meetingUpdateStatus = 1, //无更新
                    userID = userVo.userID
                }))
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        public static Status deleteMultipe(string userName,List<int> meetingIDs)
        {
            UserDAO userDao = Factory.getUserDAOInstance();
            UserVO userVo = userDao.getUserByUserName(userName);

            //检查该会议的userID与userVo中的userID是否相同，如果不相同，则无权限，立即返回

            //检查某一会议的状态，如果为2：已开，16：已结束，则失败，立即返回。

            //注意调用会议中其他内容的服务，级联删除。

            return Status.SUCCESS;
        }
    }
}