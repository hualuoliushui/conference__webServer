using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;


namespace WebServer.Models
{
    public class Organizor
    {
        private MeetingVO meetingVo;

        /// <summary>
        /// 初始化操作某一个会议
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public bool meeting_initOperator(int meetingID)
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(meetingID);
            if (meetingVo == null)
            {
                return false;
            }
            else
            {
                this.meetingVo = meetingVo;
                return true;
            }
        }
        /// <summary>
        /// 验证当前用户拥有该会议
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public  bool meeting_validatePermission(string userName)
        {
            PersonDAO personDao = Factory.getInstance<PersonDAO>();
           
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
           
            if (this.meetingVo == null)
                return false;

            wherelist.Add("personName", userName);
            PersonVO personVo = personDao.getOne<PersonVO>(wherelist);

            if (personVo == null)
            {
                return false;
            }

            if (this.meetingVo.personID == personVo.personID)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 判断会议是否 未开启 meetingStatus==1
        /// </summary>
        /// <returns></returns>
        public  bool meeting_isNotOpen()
        {
            if (meetingVo != null && meetingVo.meetingStatus == 1) 
                return true;
            return false;
        }

        /// <summary>
        /// 判断会议是否 正在开启 meetingStatus==2
        /// </summary>
        /// <returns></returns>
        public  bool meeting_isOpening()
        {
            if (meetingVo != null && meetingVo.meetingStatus == 2)
                return true;
            return false;
        }
        
        /// <summary>
        ///  判断会议是否 已结束 meetingStatus==2
        /// </summary>
        /// <returns></returns>
        public  bool meeting_isOpended()
        {
            if (meetingVo != null && meetingVo.meetingStatus == 16)
                return true;
            return false;
        }
        /// <summary>
        /// 更新“会议中，议程更新状态“
        /// </summary>
        /// <returns></returns>
        public  bool meeting_updateAgenda()
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            Dictionary<string, object> setlist = new Dictionary<string, object>();
            setlist.Add("agendaUpdateStatus", 1);
            if (meetingDao.update(setlist, meetingVo.meetingID) == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新“会议中，参会人员更新状态“
        /// </summary>
        /// <returns></returns>
        public bool meeting_updateDelegate()
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            Dictionary<string, object> setlist = new Dictionary<string, object>();
            setlist.Add("delegateUpdateStatus", 1);
            if (meetingDao.update(setlist, meetingVo.meetingID) == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新“会议中,附件更新状态“
        /// </summary>
        /// <returns></returns>
        public bool meeting_updatefile()
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            Dictionary<string, object> setlist = new Dictionary<string, object>();
            setlist.Add("fileUpdateStatus", 1);
            if (meetingDao.update(setlist, meetingVo.meetingID) == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新“会议中，表决更新状态“
        /// </summary>
        /// <returns></returns>
        public bool meeting_updatevote()
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            Dictionary<string, object> setlist = new Dictionary<string, object>();
            setlist.Add("voteUpdateStatus", 1);
            if (meetingDao.update(setlist, meetingVo.meetingID) == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 表决是否 未开启 voteStatus==1
        /// </summary>
        /// <param name="voteStatus"></param>
        /// <returns></returns>
        public bool IsNotOpen_Vote(int voteStatus)
        {
            if (voteStatus == 1)
                return true;
            return false;
        }
    }
}