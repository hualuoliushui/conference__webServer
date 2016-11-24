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
    public class MeetingService : Organizor
    {
        private static int MeetingNameMin = 2;
        private static int MeetingNameMax = 16;

        private static int MeetingSummaryMax = 50;

        //检查参数格式
        private bool checkFormat(string meetingName, string meetingSummary, DateTime meetingToStartTime)
        {
            return (meetingName.Length >= MeetingNameMin
                && meetingName.Length <= MeetingNameMax
                && meetingSummary.Length <= MeetingSummaryMax
                && (meetingToStartTime - DateTime.Now).TotalSeconds < 0 );  //时间：必须是未来
        }

        public Status create(string userName, CreateMeeting meeting)
        {
            //修正字符串
            meeting.meetingName = meeting.meetingName.Trim();
            meeting.meetingSummary = meeting.meetingSummary.Trim();
            //检查参数格式
            if (!checkFormat(meeting.meetingName, meeting.meetingSummary, meeting.meetingToStartTime))
            {
                return Status.FORMAT_ERROR;
            }

            PersonDAO personDao = Factory.getInstance < PersonDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            wherelist.Add("personName", userName);
            PersonVO personVo = personDao.getOne<PersonVO>(wherelist);

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            int meetingID = MeetingDAO.getID();
            if (meetingDao.insert<MeetingVO>(
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
                    personID = personVo.personID
                }) != 1)
            {
                return Status.FAILURE;
            }


            return Status.SUCCESS;
        }

        public Status getAll(out List<TipMeeting> meetings)
        {
            meetings = new List<TipMeeting>();

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();

            List<MeetingVO> meetingVolist = meetingDao.getAll<MeetingVO>();

            if (meetingVolist == null)
            {
                return Status.NONFOUND;
            }

            foreach (MeetingVO meetingVo in meetingVolist)
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

        /// <summary>
        /// 展示指定会议
        /// </summary>
        /// <param name="meetingID"></param>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public Status getOne(int meetingID,out Meeting meeting){
            meeting = new Meeting();

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(meetingID);

            if (meetingVo == null)
            {
                return Status.NONFOUND;
            }

            MeetingPlaceDAO meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();
            MeetingPlaceVO meetingPlaceVo = meetingPlaceDao.
                getOne<MeetingPlaceVO>(meetingVo.meetingPlaceID);

            meeting.meetingID = meetingVo.meetingID;
            meeting.meetingName = meetingVo.meetingName;
            meeting.meetingPlaceName = meetingPlaceVo.meetingPlaceName;
            meeting.meetingSummary = meetingVo.meetingSummary;
            meeting.meetingToStartTime = meetingVo.meetingToStartTime;
            meeting.meetingStatus = meetingVo.meetingStatus;

            return Status.SUCCESS;
        }

        /// <summary>
        /// 显示指定会议，用于更新
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="meetingID"></param>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public Status getOneForUpdate(string userName,int meetingID, out UpdateMeeting meeting)
        {
            meeting = new UpdateMeeting();

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(meetingID);

            if (meetingVo == null)
            {
                return Status.NONFOUND;
            }

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            //验证当前用户是否为会议拥有者
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            wherelist.Add("personName", userName);
            wherelist.Add("personID", meetingVo.personID);
            PersonVO personVo = personDao.getOne<PersonVO>(wherelist);

            if (personVo == null)
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

        /// <summary>
        /// 更新会议信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public Status update(string userName, UpdateMeeting meeting)
        {
            //修正字符串
            meeting.meetingName = meeting.meetingName.Trim();
            meeting.meetingSummary = meeting.meetingSummary.Trim();
            //检查参数格式
            if (!checkFormat(meeting.meetingName, meeting.meetingSummary, meeting.meetingToStartTime))
            {
                return Status.FORMAT_ERROR;
            }

            //验证当前用户的更新当前会议权限
            if (!validateMeeting(userName, meeting.meetingID))
            {
                return Status.PERMISSION_DENIED;
            }

            //获取会议状态
            int meetingStatus = getMeetingStatus(meeting.meetingID);
            //判断会议是否开启，如果开启，更新“会议更新状态”
            if (IsOpening_Meeting(meetingStatus))
            {
                updateMeetingUpdateStatus(meeting.meetingID);
            }
            else if (IsOpended_Meeting(meetingStatus))//如果会议已结束，直接退出
            {
                return Status.FAILURE;
            }

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(meeting.meetingID);
           
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            wherelist.Add("meetingName", meeting.meetingName);
            wherelist.Add("meetingPlace", meeting.meetingPlaceID);
            wherelist.Add("meetingSummary", meeting.meetingSummary);
            wherelist.Add("meetingToStartTime", meeting.meetingToStartTime);
           
            if ( meetingDao.update(wherelist,meeting.meetingID)!= 1)
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        public Status deleteMultipe(string userName,List<int> meetingIDs)
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();

            if (meetingIDs == null && meetingIDs.Count == 0)
            {
                return Status.SUCCESS;
            }

            foreach (int meetingID in meetingIDs)
            {
                //验证权限
                if (validateMeeting(userName, meetingID))//有权限就删除
                {
                    //获取会议状态
                    int meetingStatus = getMeetingStatus(meetingID);
                    //判断会议是否 未开启,如果 不是”未开启“，直接退出
                    if (!IsNotOpen_Meeting(meetingStatus))
                    {
                        return Status.FAILURE;
                    }
                    //注意调用会议中其他内容的服务，级联删除。
                    //删除议程

                    //删除参会人员

                    //删除当前会议
                    if (meetingDao.delete(meetingID) != 1)
                    {
                        continue;//如果失败，跳过
                    }
                }
                else // 无权限，直接退出（已删除的不恢复）
                {
                    return Status.PERMISSION_DENIED;
                } 
            }
            return Status.SUCCESS;
        }
    }
}