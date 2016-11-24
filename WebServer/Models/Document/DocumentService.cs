using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;

namespace WebServer.Models.Document
{
    public class DocumentService : Organizor
    {
        /// <summary>
        /// 获取指定议程的附件信息
        /// </summary>
        /// <param name="agendaID"></param>
        /// <param name="documents"></param>
        /// <returns></returns>
        public Status getAll(int agendaID, out List<Document> documents)
        {
            documents = new List<Document>();

            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            FileDAO fileDao = Factory.getInstance<FileDAO>();
            wherelist.Add("agendaID", agendaID);
            List<FileVO> fileVolist = fileDao.getAll<FileVO>(wherelist);
            if (wherelist == null)
            {
                return Status.NONFOUND;
            }
            foreach(FileVO fileVo in fileVolist){
                documents.Add(
                    new Document
                    {
                        documentID = fileVo.fileID,
                        documentName = fileVo.fileName,
                        documentSize = fileVo.fileSize + "KB",
                        documentPath = fileVo.filePath,
                        agendaID = fileVo.agendaID
                    });
            }

            return Status.SUCCESS;
        }

        public Status deleteMultipe(string userName, List<int> documents)
        {
            if (documents == null || documents.Count == 0)
            {
                return Status.ARGUMENT_ERROR;
            }

            FileDAO fileDao = Factory.getInstance<FileDAO>();
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            foreach (int documentID in documents)
            {
                //获取附件信息
                FileVO fileVo = fileDao.getOne<FileVO>(documentID);
                if (fileVo == null)
                    continue;

                //获取议程信息
                AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(fileVo.agendaID);
                if (agendaVo == null)
                {
                    continue;
                }

                //验证权限
                if (validateMeeting(userName, agendaVo.meetingID))//有权限就删除
                {
                    //获取会议状态
                    int meetingStatus = getMeetingStatus(agendaVo.meetingID);
                    //判断会议是否 未开启,如果 不是”未开启“，直接退出
                    if (!IsNotOpen_Meeting(meetingStatus))
                    {
                        return Status.FAILURE;
                    }
                    //更新其他附件的序号信息
                    fileDao.updateIndex(fileVo.agendaID, fileVo.fileIndex);
                    //删除当前附件
                    fileDao.delete(fileVo.fileID);
                }
                else // 无权限，直接退出（已删除的不恢复）
                {
                    return Status.PERMISSION_DENIED;
                }   

            }
            return Status.SUCCESS;
        }

        /// <summary>
        /// 为议程提供服务，删除议程下所有附件
        /// </summary>
        /// <param name="agendaID"></param>
        /// <returns></returns>
        public Status deleteAll(int agendaID)
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            FileDAO fileDao = Factory.getInstance<FileDAO>();
            wherelist.Add("agendaID", agendaID);
            fileDao.delete(wherelist);
            return Status.SUCCESS;
        }

        /// <summary>
        /// 删除指定路径的文件？？
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static Status deleteFile(string filePath)
        {
            return Status.SUCCESS;
        }

        //文件的上传和下载
    }
}