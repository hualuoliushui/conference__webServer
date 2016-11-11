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

namespace WebServer.Models.MeetingPlace
{
    public class MeetingPlaceService
    {
        private static int MeetingPlaceNameMin = 2;
        private static int MeetingPlaceNameMax = 20;

        /// <summary>
        /// 当前数据库中会场ID最大值
        /// </summary>
        private static int MeetingPlaceIDMax = Factory.getMeetingPlaceDAOInstance().getMeetingPlaceIDMax();

        private int getMeetingPlaceID()
        {
            int id = 0;
            object lockObject = new object();
            lock (lockObject)
            {
                id = ++MeetingPlaceIDMax;
            }
            return id;
        }

        private static bool checkCreateMeetingPlace(ref CreateMeetingPlace meetingPlace){
            //检查会场名称和容量参数
            if (String.IsNullOrWhiteSpace(meetingPlace.meetingPlaceName) || meetingPlace.meetingPlaceCapacity <= 0)
            {
                return false;
            }
            //修正会场名称
            meetingPlace.meetingPlaceName = meetingPlace.meetingPlaceName.Trim();

            //检查会场名称的长度是否符合要求
            if(meetingPlace.meetingPlaceName.Length < MeetingPlaceNameMin ||
                meetingPlace.meetingPlaceName.Length > MeetingPlaceNameMax)
            {
                return false;
            }

            return true;
        }

        private static bool checkUpdateMeetingPlace(ref UpdateMeetingPlace meetingPlace)
        {
            //检查会场名称和容量参数
            if (String.IsNullOrWhiteSpace(meetingPlace.meetingPlaceName) || meetingPlace.meetingPlaceCapacity <= 0)
            {
                return false;
            }
            //修正会场名称
            meetingPlace.meetingPlaceName = meetingPlace.meetingPlaceName.Trim();

            //检查会场名称的长度是否符合要求
            if (meetingPlace.meetingPlaceName.Length < MeetingPlaceNameMin ||
                meetingPlace.meetingPlaceName.Length > MeetingPlaceNameMax)
            {
                return false;
            }

            return true;
        }
 
        public Status create(CreateMeetingPlace meetingPlace)
        {
            //检查并更正格式
            if (!checkCreateMeetingPlace(ref meetingPlace))
            {
                return Status.FORMAT_ERROR;
            }

            //获取新会场ID
            int meetingPlaceID = getMeetingPlaceID();

            MeetingPlaceDAO meetingPlaceDao = Factory.getMeetingPlaceDAOInstance();
            if (!meetingPlaceDao.addMeetingPlace(
                new MeetingPlaceVO {
                    meetingPlaceID = meetingPlaceID,
                    meetingPlaceName = meetingPlace.meetingPlaceName,
                    meetingPlaceType = 0,
                    meetingPlaceCapacity = meetingPlace.meetingPlaceCapacity,
                    meetingPlaceAvailable = 0
                }))
            {
                return Status.FAILURE;
            }
            return Status.SUCCESS;
        }

        public static Status getAll(out List<MeetingPlace> meetingPlaces)
        {
            meetingPlaces = new List<MeetingPlace>();

            MeetingPlaceDAO meetingPlaceDao = Factory.getMeetingPlaceDAOInstance();

            List<MeetingPlaceVO> meetingPlaceVos = meetingPlaceDao.getMeetingPlaceList();
            if (meetingPlaceVos.Count == 0)
            {
                return Status.NONFOUND;
            }
            foreach (MeetingPlaceVO vo in meetingPlaceVos)
            {
                meetingPlaces.Add(new MeetingPlace
                {
                    meetingPlaceID = vo.meetingPlaceID,
                    meetingPlaceName = vo.meetingPlaceName,
                    meetingPlaceCapacity = vo.meetingPlaceCapacity,
                    meetingPlaceFreezeState = vo.meetingPlaceAvailable
                });
            }

            return Status.SUCCESS;
        }

        public static Status getOne(out UpdateMeetingPlace meetingPlace, int meetingPlaceID)
        {
            meetingPlace = new UpdateMeetingPlace();

            MeetingPlaceDAO meetingPlaceDao = Factory.getMeetingPlaceDAOInstance();
            MeetingPlaceVO vo = meetingPlaceDao.getMeetingPlaceByMeetingPlaceID(meetingPlaceID);

            if (vo == null)
            {
                return Status.NONFOUND;
            }
            meetingPlace.meetingPlaceID = vo.meetingPlaceID;
            meetingPlace.meetingPlaceName = vo.meetingPlaceName;
            meetingPlace.meetingPlaceCapacity = vo.meetingPlaceCapacity;

            return Status.SUCCESS;
        }

        public static Status update(UpdateMeetingPlace meetingPlace)
        {
            //检查并更正格式
            if (!checkUpdateMeetingPlace(ref meetingPlace))
            {
                return Status.FORMAT_ERROR;
            }

            MeetingPlaceDAO meetingPlaceDao = Factory.getMeetingPlaceDAOInstance();
            if (!meetingPlaceDao.updateMeetingPlace(
                new MeetingPlaceVO { 
                    meetingPlaceID = meetingPlace.meetingPlaceID,
                    meetingPlaceName = meetingPlace.meetingPlaceName, 
                    meetingPlaceType = 0, 
                    meetingPlaceCapacity = meetingPlace.meetingPlaceCapacity
                }))
            {
                return Status.FAILURE;
            }
                return Status.SUCCESS;
        }

        public static Status UpdateUserAvailable(int meetingPlaceID,int available){
            //检查参数
            if (meetingPlaceID < 0
                || (available != 0 && available != 1))
            {
                return Status.ARGUMENT_ERROR;
            }

            //数据库操作
            MeetingPlaceDAO meetingPlaceDao = Factory.getMeetingPlaceDAOInstance();
            if (!meetingPlaceDao.updateMeetingPlaceAvailable(meetingPlaceID,available))
            {
                return Status.FAILURE;
            }
            return Status.SUCCESS;
        }
    }
}