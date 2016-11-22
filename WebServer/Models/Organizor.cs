using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;


namespace WebServer.Models
{
    public class Organizor
    {
        /// <summary>
        /// 验证当前用户拥有该会议
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public static bool validateMeeting(string userName, int meetingID)
        {
            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(meetingID);
            if (meetingVo == null)
                return false;

            wherelist.Add("personName", userName);
            PersonVO personVo = personDao.getOne<PersonVO>(wherelist);

            if (personVo == null)
            {
                return false;
            }

            if (meetingVo.personID == personVo.personID)
                return true;
            else
                return false;
        }

        public static int getMeetingStatus(int meetingID)
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(meetingID);
            if (meetingVo == null)
                return -1;
            return meetingVo.meetingStatus;
        }

        /// <summary>
        /// 判断会议是否 未开启 meetingStatus==1
        /// </summary>
        /// <param name="meetingStatus"></param>
        /// <returns></returns>
        public static bool IsNotOpen_Meeting(int meetingStatus)
        {
            if (meetingStatus == 1)
                return true;
            return false;
        }

        /// <summary>
        /// 判断会议是否 正在开启 meetingStatus==2
        /// </summary>
        /// <param name="meetingStatus"></param>
        /// <returns></returns>
        public static bool IsOpening_Meeting(int meetingStatus)
        {
            if (meetingStatus == 2)
                return true;
            return false;
        }
        
        /// <summary>
        ///  判断会议是否 已结束 meetingStatus==2
        /// </summary>
        /// <param name="meetingStatus"></param>
        /// <returns></returns>
        public static bool IsOpended_Meeting(int meetingStatus)
        {
            if (meetingStatus == 16)
                return true;
            return false;
        }
        /// <summary>
        /// 更新“会议更新状态“
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public static bool updateMeetingUpdateStatus(int meetingID)
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            Dictionary<string, object> setlist = new Dictionary<string, object>();
            setlist.Add("meetingUpdateStatus", 1);
            if (meetingDao.update(setlist, meetingID) == 1)
            {
                return true;
            }
            return false;
        }
    }
}