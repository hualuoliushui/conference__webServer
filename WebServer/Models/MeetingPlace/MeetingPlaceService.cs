using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DAL.DAOVO;

namespace WebServer.Models.MeetingPlace
{
    public class MeetingPlaceService
    {
        public static int create(NewMeetingPlace newMeetingPlace)
        {
            //执行数据库插入操作

            return 1;
        }

        public static int getAll(out MeetingPlaces meetingPlaces)
        {
            meetingPlaces = new MeetingPlaces();
            meetingPlaces.meetingPlaces = new List<MeetingPlaceVO>();


            //查询数据
            meetingPlaces.meetingPlaces.Add(new MeetingPlaceVO { meetingPlaceID = 1, meetingPlaceName = "人民大会堂", meetingPlaceType = 1, meetingPlaceCapacity = 200 });
            meetingPlaces.meetingPlaces.Add(new MeetingPlaceVO { meetingPlaceID = 2, meetingPlaceName = "学术大讲堂", meetingPlaceType = 2, meetingPlaceCapacity = 100 });

            return 1;
        }

        public static int update(MeetingPlaces meetingPlaces)
        {
            //更新数据

            return 1;
        }

        public static int delete(OldMeetingPlaces meetingPlaces)
        {
            //删除数据

            return 1;
        }

    }
}