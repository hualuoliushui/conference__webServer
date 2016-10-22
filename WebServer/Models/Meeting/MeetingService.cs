using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Meeting
{
    public class MeetingService
    {
        public static int create(NewMeeting newMeeting)
        {
            //执行数据库插入操作

            return 1;
        }

        public static int getAll(out Meetings meetings)
        {
            meetings = new Meetings();
            meetings.meetings = new List<Meeting>();


            //查询数据

            return 1;
        }

        public static int update(Meetings meetings)
        {
            //更新数据

            return 1;
        }

        public static int delete(OldMeetings meetings)
        {
            //删除数据

            return 1;
        }
    }
}