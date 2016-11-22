using System.Collections.Generic;
using DAL.DAOVO;
using DAL.DAO;
using DAL.DAOFactory;


namespace WebServer.Models.MeetingPlace
{
    public class MeetingPlaceService
    {
        private static int MeetingPlaceNameMin = 2;
        private static int MeetingPlaceNameMax = 20;

        //检查会场名称的长度是否符合要求
        private static bool checkFormat(string meetingPlaceName){

            return (meetingPlaceName.Length >= MeetingPlaceNameMin &&
                meetingPlaceName.Length <= MeetingPlaceNameMax);
        }

        public Status create(CreateMeetingPlace meetingPlace)
        {
            //修正字符串
            meetingPlace.meetingPlaceName = meetingPlace.meetingPlaceName.Trim();
            //检查长度规范
            if (!checkFormat(meetingPlace.meetingPlaceName))
            {
                return Status.FORMAT_ERROR;
            }

            //获取新会场ID
            int meetingPlaceID = MeetingDAO.getID();

            MeetingPlaceDAO meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();
            if (meetingPlaceDao.insert<MeetingPlaceVO>(
                new MeetingPlaceVO {
                    meetingPlaceID = meetingPlaceID,
                    meetingPlaceName = meetingPlace.meetingPlaceName,
                    meetingPlaceCapacity = meetingPlace.meetingPlaceCapacity,
                    meetingPlaceState = 0
                }) != 1 )
            {
                return Status.FAILURE;
            }
            return Status.SUCCESS;
        }

        public static Status getAllForMeeting(out List<MeetingPlaceForMeeting> meetingPlaces)
        {
            meetingPlaces = new List<MeetingPlaceForMeeting>();

            MeetingPlaceDAO meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            wherelist.Add("meetingPlaceState", 0);
            //获取未冻结的会场信息
            List<MeetingPlaceVO> meetingPlaceVos = meetingPlaceDao.getAll<MeetingPlaceVO>(wherelist);
        
            if (meetingPlaceVos == null)
            {
                return Status.NONFOUND;
            }

            foreach (MeetingPlaceVO vo in meetingPlaceVos)
            {
                meetingPlaces.Add(
                    new MeetingPlaceForMeeting
                    {
                        meetingPlaceID = vo.meetingPlaceID,
                        meetingPlaceName = vo.meetingPlaceName
                    });
            }



            return Status.SUCCESS;
        }

        public static Status getAll(out List<MeetingPlace> meetingPlaces)
        {
            meetingPlaces = new List<MeetingPlace>();

            MeetingPlaceDAO meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();

            List<MeetingPlaceVO> meetingPlaceVos = meetingPlaceDao.getAll<MeetingPlaceVO>();
            if (meetingPlaceVos == null)
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
                    meetingPlaceFreezeState = vo.meetingPlaceState
                });
            }

            return Status.SUCCESS;
        }

        public static Status getOneForUpdate(out UpdateMeetingPlace meetingPlace, int meetingPlaceID)
        {
            meetingPlace = new UpdateMeetingPlace();

            MeetingPlaceDAO meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();
            MeetingPlaceVO vo = meetingPlaceDao.getOne<MeetingPlaceVO>(meetingPlaceID);

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

            //修正字符串
            meetingPlace.meetingPlaceName = meetingPlace.meetingPlaceName.Trim();
            //检查长度规范
            if (!checkFormat(meetingPlace.meetingPlaceName))
            {
                return Status.FORMAT_ERROR;
            }

            Dictionary<string, object> setlist = new Dictionary<string, object>();

            MeetingPlaceDAO meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();

            setlist.Add("meetingPlaceName", meetingPlace.meetingPlaceName);
            setlist.Add("meetingPlaceCapacity", meetingPlace.meetingPlaceCapacity);
            if (meetingPlaceDao.update(setlist,meetingPlace.meetingPlaceID) != 1 )
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
            Dictionary<string, object> setlist = new Dictionary<string, object>();

            MeetingPlaceDAO meetingPlaceDao = Factory.getInstance<MeetingPlaceDAO>();

            setlist.Add("meetingPlaceState", available);

            if (meetingPlaceDao.update(setlist,meetingPlaceID) != 1)
            {
                return Status.FAILURE;
            }
            return Status.SUCCESS;
        }

       
    }
}