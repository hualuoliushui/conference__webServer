using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;

namespace WebServer.Models.LongTable
{
    public class LongTableService : Organizor
    {
        public Status create(CreateLongTable createLongTable)
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            //wherelist.Add("meetingPlaceID", createLongTable.meetingPlaceID);
            //if (!unique<LongTableDAO, LongTableVO>(wherelist))
            //{
            //    return Status.NAME_EXIST;
            //}

            LongTableDAO longTableDao = Factory.getInstance<LongTableDAO>();

            int longTableID = LongTableDAO.getID();

            if (longTableDao.insert<LongTableVO>(
                new LongTableVO
                {
                    longTableID = longTableID,
                    upNum = createLongTable.upNum,
                    downNum = createLongTable.downNum,
                    leftNum = createLongTable.leftNum,
                    rightNum = createLongTable.rightNum,
                    meetingPlaceID = createLongTable.meetingPlaceID
                }) < 0)
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        public Status delete(int meetingPlaceID)
        {
            LongTableDAO longTableDao = Factory.getInstance<LongTableDAO>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            wherelist.Add("meetingPlaceID", meetingPlaceID);

            longTableDao.delete(wherelist);

            return Status.SUCCESS;
        }

        public Status update(UpdateLongTable updateLongTable)
        {
            LongTableDAO longTableDao = Factory.getInstance<LongTableDAO>();

            LongTableVO longTableVo = longTableDao.getOne<LongTableVO>(updateLongTable.longTableID);
            if (longTableVo == null)
            {
                return Status.NONFOUND;
            }

            Dictionary<string, object> setlist = new Dictionary<string, object>();

            setlist.Add("upNum", updateLongTable.upNum);
            setlist.Add("downNum", updateLongTable.downNum);
            setlist.Add("leftNum", updateLongTable.leftNum);
            setlist.Add("rightNum", updateLongTable.rightNum);

            if (longTableDao.update(setlist, updateLongTable.longTableID) < 0)
            {
                return Status.FAILURE;
            }

            return Status.SUCCESS;
        }

        public Status getOneForUpdate(int meetingPlaceID, out UpdateLongTable longTable)
        {
            longTable = new UpdateLongTable();

            LongTableDAO longTableDao = Factory.getInstance<LongTableDAO>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            wherelist.Add("meetingPlaceID", meetingPlaceID);
            LongTableVO longTableVo = longTableDao.getOne<LongTableVO>(wherelist);

            if (longTableVo == null)
            {
                return Status.NONFOUND;
            }

            longTable.longTableID = longTableVo.longTableID;
            longTable.upNum = longTableVo.upNum;
            longTable.downNum = longTableVo.downNum;
            longTable.leftNum = longTableVo.leftNum;
            longTable.rightNum = longTableVo.rightNum;

            return Status.SUCCESS;
        }

    }
}