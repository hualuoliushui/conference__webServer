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
using WebServer.App_Start;
using WebServer.Models.Agenda;
using WebServer.Models.Delegate;

namespace WebServer.Models.Meeting
{
    public class MeetingService : Organizor
    {

        //检查参数格式
        private bool checkFormat(DateTime meetingToStartTime, DateTime meetingStartedTime)
        {
            return (
                (meetingToStartTime - DateTime.Now).TotalSeconds >= 0 //时间：必须是未来
                && (meetingStartedTime - meetingToStartTime).TotalSeconds >= 0);    //结束时间必须晚于开始时间

        }

        public Status create(ref MeetingInfo meeting)
        {
            //修正字符串
            meeting.meetingName = meeting.meetingName.Trim();
            meeting.meetingSummary = meeting.meetingSummary.Trim();
            //检查参数格式
            if (!checkFormat(meeting.meetingToStartTime,meeting.meetingStartedTime))
            {
                return Status.TIME_SET_ERROR;
            }

            PersonDAO personDao = Factory.getInstance < PersonDAO>();
           
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            meeting.meetingID = MeetingDAO.getID();
            if (meetingDao.insert<MeetingVO>(
                new MeetingVO
                {
                    meetingID = meeting.meetingID,
                    meetingName = meeting.meetingName,
                    meetingPlaceID = meeting.meetingPlaceID,
                    meetingSummary = meeting.meetingSummary,
                    meetingToStartTime = meeting.meetingToStartTime,
                    meetingStatus = 1,//未开
                    meetingDuration = 0,
                    meetingStartedTime = meeting.meetingStartedTime,
                    delegateUpdateStatus = 0,//无更新
                    agendaUpdateStatus = 0, //无更新
                    fileUpdateStatus = 0, //无更新
                    voteUpdateStatus = 0, //无更新
                    personID = 1 //设置为超级管理员
                }) < 0)
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

            if (meetingVolist != null)
            {
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
            }
            return Status.SUCCESS;
        }

        /// <summary>
        /// 展示指定会议
        /// </summary>
        /// <param name="meetingID"></param>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public Status getOne(int meetingID,out MeetingInfo meeting){
            meeting = new MeetingInfo();

            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(meetingID);

            if (meetingVo != null)
            {
                meeting.meetingID = meetingVo.meetingID;
                meeting.meetingName = meetingVo.meetingName;
                meeting.meetingPlaceID = meetingVo.meetingPlaceID;
                meeting.meetingSummary = meetingVo.meetingSummary;
                meeting.meetingStartedTime = meetingVo.meetingStartedTime;
                meeting.meetingToStartTime = meetingVo.meetingToStartTime;
                meeting.meetingStatus = meetingVo.meetingStatus;

                return Status.SUCCESS;
            }

            return Status.NONFOUND;
        }

        /// <summary>
        /// 更新会议信息
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public Status update(MeetingInfo meeting)
        {
            if(string.IsNullOrWhiteSpace(meeting.meetingName)
                || string.IsNullOrWhiteSpace(meeting.meetingSummary))
            {
                return Status.ARGUMENT_ERROR;
            }
            //修正字符串
            meeting.meetingName = meeting.meetingName.Trim();
            meeting.meetingSummary = meeting.meetingSummary.Trim();
            //检查时间参数
            if (!checkFormat( meeting.meetingToStartTime,meeting.meetingStartedTime))
            {
                return Status.TIME_SET_ERROR;
            }


            //初始化会议操作
            meeting_initOperator(meeting.meetingID);

            //判断会议是否开启，如果正在开启，直接退出
            if (meeting_isOpening())
            {
                return Status.MEETING_OPENING;
            }
            else if (meeting_isOpended())//如果会议已结束，直接退出
            {
                return Status.FAILURE;
            }
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();
            MeetingVO meetingVo = meetingDao.getOne<MeetingVO>(meeting.meetingID);
         
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            
            wherelist.Add("meetingName", meeting.meetingName);
            wherelist.Add("meetingPlaceID", meeting.meetingPlaceID);
            wherelist.Add("meetingSummary", meeting.meetingSummary);
            wherelist.Add("meetingToStartTime", meeting.meetingToStartTime);
            wherelist.Add("meetingStartedTime", meeting.meetingStartedTime);

            int num = meetingDao.update(wherelist,meeting.meetingID);
            if ( num < 0)
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        public Status deleteMultipe(int meetingID)
        {
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();

            //初始化会议操作
            meeting_initOperator(meetingID);

            //判断会议是否 未开启,如果 不是”未开启“，直接退出
            if (!meeting_isNotOpen())
            {
                return Status.MEETING_OPENED;
            }
            AgendaService agendaService = new AgendaService();
            DelegateService delegateService = new DelegateService();

            //注意调用会议中其他内容的服务，级联删除。
            //删除议程
            agendaService.deleteAll(meetingID);
            //删除参会人员
            delegateService.deleteAll(meetingID);
            //删除当前会议
            int checkNum = meetingDao.delete(meetingID);
         
            return Status.SUCCESS;
        }
    }
}