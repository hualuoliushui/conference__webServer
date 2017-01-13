using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;
using WebServer.Models.Document;
using WebServer.App_Start;
using WebServer.Models.Vote;

namespace WebServer.Models.Agenda
{
    public class AgendaService : Organizor
    {
        private static int AgendaNameMin = 2;
        private static int AgendaNameMax = 16;


        //检查参数格式
        private bool checkFormat(string agendaName,int agendaDuration)
        {
            return ( agendaName.Length >= AgendaNameMin 
                && agendaName.Length <= AgendaNameMax
                && agendaDuration > 0
                && agendaDuration < 2147483647); // 议程时长必需大于0,小于2的31次方减1
        }


        //获取指定会议的议程
        public Status getAll(int meetingID, out List<AgendaInfo> agendas)
        {
            agendas = new List<AgendaInfo>();

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            PersonDAO personDao = Factory.getInstance<PersonDAO>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            //获取指定会议的议程
            wherelist.Add("meetingID", meetingID);
            List<AgendaVO> AgendaVolist = agendaDao.getAll<AgendaVO>(wherelist);

            if (AgendaVolist == null)
            {
                return Status.SUCCESS;
            }

            foreach (AgendaVO agendaVo in AgendaVolist)
            {
                //获取主讲人信息
                PersonVO personVo = personDao.getOne<PersonVO>(agendaVo.personID);
                if (personVo == null )
                {
                    return Status.FAILURE;
                }
                //将信息插入到返回信息中
                agendas.Add(
                    new AgendaInfo
                    {
                        agendaID = agendaVo.agendaID,
                        agendaName =agendaVo.agendaName,
                        agendaDuration = agendaVo.agendaDuration,
                        userName = personVo.personName,
                        meetingID = agendaVo.meetingID
                    });
            }

            return Status.SUCCESS;
        }

        public Status getOne(int agendaID, out AgendaInfo agenda)
        {
            agenda = new AgendaInfo();

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(agendaID);

            if (agendaVo == null)
            {
                return Status.NONFOUND;
            }

            PersonDAO personDao = Factory.getInstance<PersonDAO>();
            PersonVO personVo = personDao.getOne<PersonVO>(agendaVo.personID);
            if (personVo == null)
            {
                return Status.FAILURE;
            }

            agenda.agendaID = agendaVo.agendaID;
            agenda.agendaName = agendaVo.agendaName;
            agenda.agendaDuration = agendaVo.agendaDuration;
            agenda.userName = personVo.personName;
            agenda.meetingID = agendaVo.meetingID;

            return Status.SUCCESS;
        }


        public Status create(CreateAgenda createAgenda)
        {
            if(string.IsNullOrWhiteSpace(createAgenda.agendaName))
            {
                return Status.ARGUMENT_ERROR;
            }

            //修正字符串
            createAgenda.agendaName = createAgenda.agendaName.Trim();
            //检查参数格式
            if(!checkFormat(createAgenda.agendaName,createAgenda.agendaDuration)){
                return Status.FORMAT_ERROR;
            }
            //初始化会议操作
            meeting_initOperator(createAgenda.meetingID);

            bool isUpdate = false;
            //判断会议是否开启，如果开启，更新“议程更新状态”，数据设置更新状态
            if (meeting_isOpening())
            {
                meeting_updateAgenda();
                isUpdate = true;
            }
            else if (meeting_isOpended())//如果会议已结束，直接退出
            {
                return Status.FAILURE;
            }

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            MeetingDAO meetingDao = Factory.getInstance<MeetingDAO>();

            Dictionary<string,object> wherelist = new Dictionary<string,object>();

            //获取当前议程的个数
            wherelist.Add("meetingID",createAgenda.meetingID);
            List<AgendaVO> agendaVolist = agendaDao.getAll<AgendaVO>(wherelist);

            //获取新议程的ID
            int agendaID = AgendaDAO.getID();

            //设置新的议程编号
            int agendaIndex = agendaVolist == null ? 1 : agendaVolist.Count+1;
            //添加议程
            AgendaVO agendaVo = new AgendaVO
            {
                agendaID = agendaID,
                agendaName = createAgenda.agendaName,
                agendaDuration = createAgenda.agendaDuration,
                agendaIndex = agendaIndex,
                meetingID = createAgenda.meetingID,
                personID = createAgenda.userID,
                isUpdate = isUpdate //判断是否属于会议中新加入的信息
            };

            if( agendaDao.insert<AgendaVO>(agendaVo) < 0){
                return Status.DATABASE_OPERATOR_ERROR;
            }else
            {
                if (meetingDao.increateDuration(agendaVo.meetingID, agendaVo.agendaDuration) < 0)
                {
                    agendaDao.delete(agendaID);
                    return Status.DATABASE_OPERATOR_ERROR;
                }
            }
            return Status.SUCCESS;
        }

        public Status update(UpdateAgenda updateAgenda)
        {
            if (string.IsNullOrWhiteSpace(updateAgenda.agendaName))
            {
                return Status.ARGUMENT_ERROR;
            }
            //修正字符串
            updateAgenda.agendaName = updateAgenda.agendaName.Trim();
            //检查参数格式
            if (!checkFormat(updateAgenda.agendaName, updateAgenda.agendaDuration))
            {
                return Status.FORMAT_ERROR;
            }

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(updateAgenda.agendaID);

            if (agendaVo == null)
            {
                return Status.NONFOUND;
            }

            //初始化会议操作
            meeting_initOperator(agendaVo.meetingID);

            //判断会议是否开启，如果正在开启，直接退出
            if (meeting_isOpening())
            {
                return Status.MEETING_OPENING;
            }
            else if (meeting_isOpended())//如果会议已结束，直接退出
            {
                return Status.FAILURE;
            }

            Dictionary<string, object> setlist = new Dictionary<string, object>();
            setlist.Add("agendaName", updateAgenda.agendaName);
            setlist.Add("agendaDuration", updateAgenda.agendaDuration);
            setlist.Add("personID", updateAgenda.userID);

            if (agendaDao.update(setlist, updateAgenda.agendaID) < 0)
            {
                return Status.FAILURE;
            }
            return Status.SUCCESS;
        }

        public Status deleteMultipe(List<int> agendaIDs)
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();

            if (agendaIDs == null || agendaIDs.Count == 0)
                return Status.SUCCESS;

            foreach(int agendaID in agendaIDs)
            {
                 //获取议程所属会议
                 AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(agendaID);

                if( agendaVo == null){
                   return Status.NONFOUND;
                }

                //初始化会议操作
                meeting_initOperator(agendaVo.meetingID);

                //判断会议是否 未开启,如果 不是”未开启“，直接退出
                if (!meeting_isNotOpen())
                {
                    return Status.MEETING_OPENING;
                }
                
                //修改其他议程的序号
                agendaDao.updateIndex(agendaVo.meetingID, agendaVo.agendaIndex);

                //删除该议程下的附件
                new DocumentService().deleteAll(agendaVo.agendaID);
                //删除该议程下的表决
                new VoteService().deleteAll(agendaVo.agendaID);
                //删除该议程
                agendaDao.delete(agendaID);
            }
            return Status.SUCCESS;
        }

        /// <summary>
        /// 删除会议时使用
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public Status deleteAll(int meetingID)
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            wherelist.Add("meetingID", meetingID);
            List<AgendaVO> agendaVolist = agendaDao.getAll<AgendaVO>(wherelist);
            if (agendaVolist != null)
            {
                DocumentService documentService = new DocumentService();
                VoteService voteService = new VoteService();
                foreach (AgendaVO agendaVo in agendaVolist)
                {
                    documentService.deleteAll(agendaVo.agendaID);
                    voteService.deleteAll(agendaVo.agendaID);
                }
            }
            return Status.SUCCESS;
        }
    }
}