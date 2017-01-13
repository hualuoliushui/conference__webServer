using DAL.DAO;
using DAL.DAOFactory;
using DAL.DAOVO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.App_Start;

namespace WebServer.Models.Vote
{
    public class VoteService : Organizor
    {
        private static int VoteNameMin = 1;
        private static int VoteNameMax = 50;

        private static int VoteDescriptionMax = 255;

        private static int VoteOptionMin = 1;
        private static int VoteOptionMax = 50;

        private bool checkFormat(string voteName, string voteDescription, List<string> voteOptions)
        {
            if (voteName.Length >= VoteNameMin
                && voteName.Length <= VoteNameMax
                && voteDescription.Length <= VoteDescriptionMax)
            {
                foreach (string item in voteOptions)
                {
                    if (item.Length < VoteOptionMin
                        || item.Length > VoteOptionMax)
                    {
                        return false;
                    }
                }
                return true; //所有项均符合
            }
            else
                return false;
        }

        public Status getAll(int agendaID, out List<VoteInfo> votes)
        {
            votes = new List<VoteInfo>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            VoteDAO voteDao = Factory.getInstance<VoteDAO>();
            wherelist.Add("agendaID", agendaID);
            List<VoteVO> voteVolist = voteDao.getAll<VoteVO>(wherelist);
            if (voteVolist == null)
            {
                return Status.NONFOUND;
            }
            foreach (VoteVO voteVo in voteVolist)
            {
                wherelist.Clear();

                VoteOptionDAO voteOptionDao = Factory.getInstance<VoteOptionDAO>();
                wherelist.Add("voteID", voteVo.voteID);
                List<VoteOptionVO> voteOptionVolist = voteOptionDao.getAll<VoteOptionVO>(wherelist);
                //将选项按序号排序
                voteOptionVolist.Sort((x, y) => x.voteOptionIndex - y.voteOptionIndex);

                List<String> optionList = new List<string>();
                foreach (VoteOptionVO voteOptionVo in voteOptionVolist)
                {
                    optionList.Add(voteOptionVo.voteOptionName);
                }
                votes.Add(
                    new VoteInfo
                    {
                        voteID = voteVo.voteID,
                        voteName = voteVo.voteName,
                        voteDescription = voteVo.voteDescription,
                        voteType = voteVo.voteType,
                        optionNum = optionList.Count,
                        options = optionList
                    });
            }

            return Status.SUCCESS;
        }

        public Status getOne(int voteID, out VoteInfo vote)
        {
            vote = new VoteInfo();
            try
            {
                Dictionary<string, object> wherelist = new Dictionary<string, object>();

                VoteDAO voteDao = Factory.getInstance<VoteDAO>();
                VoteOptionDAO voteOptionDao = Factory.getInstance<VoteOptionDAO>();

                VoteVO voteVo = voteDao.getOne<VoteVO>(voteID);
                if (voteVo == null)
                {
                    return Status.NONFOUND;
                }
                //获取表决选项列表
                List<string> voteOptions = new List<string>();
                wherelist.Add("voteID", voteID);
                List<VoteOptionVO> voteOptionVolist = voteOptionDao.getAll<VoteOptionVO>(wherelist);
                //将选项按序号排序
                voteOptionVolist.Sort((x, y) => x.voteOptionIndex - y.voteOptionIndex);
                //填充返回列表
                if (voteOptionVolist != null)
                {
                    foreach (VoteOptionVO voteOptionVo in voteOptionVolist)
                    {
                        voteOptions.Add(voteOptionVo.voteOptionName);
                    }
                }

                vote.agendaID = voteVo.agendaID;
                vote.voteID = voteVo.voteID;
                vote.voteName = voteVo.voteName;
                vote.voteDescription = voteVo.voteDescription;
                vote.voteType = voteVo.voteType;
                vote.optionNum = voteOptions.Count;
                vote.options = voteOptions;

                return Status.SUCCESS;
            }
            catch (Exception e)
            {
                Log.LogInfo("为更新获取表决信息", e);
                return Status.SERVER_EXCEPTION;
            }

        }

        public Status deleteMultipe(List<int> votes)
        {
            if (votes == null || votes.Count == 0)
            {
                return Status.ARGUMENT_ERROR;
            }

            VoteDAO voteDao = Factory.getInstance<VoteDAO>();
            VoteOptionDAO voteOptionDao = Factory.getInstance<VoteOptionDAO>();
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            foreach (int voteID in votes)
            {
                //获取表决信息
                VoteVO voteVo = voteDao.getOne<VoteVO>(voteID);
                if (voteVo == null)
                    continue;

                //判断投票是否未开启,如果 不是”未开启“，直接退出
                if (!IsNotOpen_Vote(voteVo.voteStatus))
                {
                    return Status.FAILURE;
                }

                //获取议程信息
                AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(voteVo.agendaID);
                if (agendaVo == null)
                {
                    continue;
                }

                //初始化会场操作
                meeting_initOperator(agendaVo.meetingID);

                //判断会议是否 未开启,如果 不是”未开启“，直接退出
                if (!meeting_isNotOpen())
                {
                    return Status.FAILURE;
                }
                //删除选项
                wherelist.Clear();
                wherelist.Add("voteID", voteID);
                voteOptionDao.delete(wherelist);

                //更新其他投票的序号信息
                voteDao.updateIndex(voteVo.agendaID, voteVo.voteIndex);
                //删除当前投票
                voteDao.delete(voteVo.voteID);
                    

            }
            return Status.SUCCESS;
        }

        /// <summary>
        /// 删除会议时使用
        /// </summary>
        /// <param name="agendaID"></param>
        /// <returns></returns>
        public Status deleteAll(int agendaID)
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            VoteDAO voteDao = Factory.getInstance<VoteDAO>();
            VoteOptionDAO voteOptionDao = Factory.getInstance<VoteOptionDAO>();
            wherelist.Add("agendaID", agendaID);

            var voteVolist = voteDao.getAll<VoteVO>(wherelist);
            if (voteVolist != null)
            {
                foreach (var voteVo in voteVolist)
                {
                    wherelist.Clear();
                    wherelist.Add("voteID", voteVo.voteID);
                    voteOptionDao.delete(wherelist);
                }
            }

            wherelist.Clear();
            wherelist.Add("agendaID", agendaID);

            voteDao.delete(wherelist);//删除表决
            return Status.SUCCESS;
        }

        public Status update(UpdateVote vote)
        {
            VoteDAO voteDao = Factory.getInstance<VoteDAO>();
            VoteVO voteVo = voteDao.getOne<VoteVO>(vote.voteID);
            if (voteVo == null)
            {
                return Status.NONFOUND;
            }

            //修正字符串
            vote.voteName = vote.voteName.Trim();
            vote.voteDescription = vote.voteDescription.Trim();

            //检查参数格式
            if (!checkFormat(vote.voteName, vote.voteDescription, vote.voteOptions))
            {
                return Status.FORMAT_ERROR;
            }

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(voteVo.agendaID);
            if (agendaVo == null)
            {
                return Status.FAILURE;
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

            // 更新vote
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            wherelist.Add("voteName", vote.voteName);
            wherelist.Add("voteDescription", vote.voteDescription);
            wherelist.Add("voteType", vote.voteType);
            if (voteDao.update(wherelist, vote.voteID) != 1)
            {
                return Status.FAILURE;
            }

            // 清空投票选项列表
            VoteOptionDAO voteOptionDao = Factory.getInstance<VoteOptionDAO>();
            Dictionary<string, object> wherelist1 = new Dictionary<string, object>();
            wherelist1.Add("voteID", vote.voteID);
            voteOptionDao.delete(wherelist1);

            // 重写投票选项列表
            int index = 1;
            foreach (string voteOption in vote.voteOptions)
            {
                int newVoteOptionID = VoteOptionDAO.getID();
                if (voteOptionDao.insert<VoteOptionVO>(
                    new VoteOptionVO
                    {
                        voteOptionID = newVoteOptionID,
                        voteOptionName = voteOption,
                        voteOptionIndex = index,
                        voteID = vote.voteID
                    }) < 0)
                {
                    return Status.FAILURE;
                }

                ++index;
            }

            return Status.SUCCESS;
        }


        public Status create(CreateVote vote)
        {
            //修正字符串
            vote.voteName = vote.voteName.Trim();
            vote.voteDescription = vote.voteDescription.Trim();

            //检查参数格式
            if (!checkFormat(vote.voteName, vote.voteDescription, vote.voteOptions))
            {
                return Status.FORMAT_ERROR;
            }

            //验证当前用户的更新当前会议权限
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(vote.agendaID);
            if (agendaVo == null)
            {
                return Status.FAILURE;
            }

            //初始化会议操作
            meeting_initOperator(agendaVo.meetingID);

            bool isUpdate = false;
            //判断会议是否开启，如果开启，更新"更新状态”,设置数据更新状态
            if (meeting_isOpening())
            {
                meeting_updatevote();
                isUpdate = true;
            }
            else if (meeting_isOpended())//如果会议已结束，直接退出
            {
                return Status.FAILURE;
            }

            // 插入投票
            VoteDAO voteDao = Factory.getInstance<VoteDAO>();
            Dictionary<string,object> wherelist = new Dictionary<string,object>();
            wherelist.Clear();
            wherelist.Add("agendaID",vote.agendaID);
            List<VoteVO> voteVolist = voteDao.getAll<VoteVO>(wherelist);

            //设置新的投票编号
            int voteIndex = voteVolist == null ? 1 : voteVolist.Count + 1;

            // 先获取新的ID
            int newVoteID = VoteDAO.getID();
            if (voteDao.insert<VoteVO>(
                    new VoteVO
                    {
                        voteID = newVoteID,
                        voteIndex = voteIndex,
                        voteName = vote.voteName,
                        voteDescription = vote.voteDescription,
                        voteType = vote.voteType,
                        voteStatus = 1, // 未开
                        agendaID = vote.agendaID,
                        isUpdate = isUpdate
                    }) < 0)
            {
                return Status.FAILURE;
            }

            // 插入投票选项列表
            int index = 1;
            VoteOptionDAO voteOptionDao = Factory.getInstance<VoteOptionDAO>();
            foreach (string voteOption in vote.voteOptions)
            {
                if (voteOptionDao.insert<VoteOptionVO>(
                    new VoteOptionVO
                    {
                        voteOptionID = VoteDAO.getID(),
                        voteOptionName = voteOption,
                        voteOptionIndex = index,
                        voteID = newVoteID
                    }) < 0 )
                {
                    return Status.FAILURE;
                }

                ++index;
            }

            return Status.SUCCESS;
        }
    }
}