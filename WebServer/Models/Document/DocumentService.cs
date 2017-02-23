using System.Collections.Generic;

using System;
using System.IO;
using System.Web;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;
using System.Configuration;
using WebServer.Models.Document.FileConvertService;
using WebServer.App_Start;

namespace WebServer.Models.Document
{
    public class DocumentService : Organizor
    {
        private string separator = Path.DirectorySeparatorChar.ToString();

        private string GetSaveDir()
        {
            string saveDir = ConfigurationManager.AppSettings["SaveDir"];

            if (string.IsNullOrEmpty(saveDir))
            {
                saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload");
            }

            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }
            return saveDir;
        }

        private string GetHtmlDir()
        {
            string htmlDir = ConfigurationManager.AppSettings["HtmlDir"];
            if (string.IsNullOrEmpty(htmlDir))
            {
                htmlDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Download");     
            }

            if (!Directory.Exists(htmlDir))
            {
                Directory.CreateDirectory(htmlDir);
            }
            return htmlDir;
        }

        private string GetHtmlRelativeDir()
        {
            string htmlRelativeDir = ConfigurationManager.AppSettings["htmlRelativeDir"];

            if (string.IsNullOrEmpty(htmlRelativeDir))
            {
                htmlRelativeDir = separator + "Download" + separator;
            }

            return htmlRelativeDir;
        }

        private bool checkFileFormat(string fileName)
        {
            fileName = fileName.ToLower();

            if(fileName.EndsWith(".doc")
                || fileName.EndsWith(".docx")
                || fileName.EndsWith(".xls")
                || fileName.EndsWith(".xlsx")
                || fileName.EndsWith(".ppt")
                || fileName.EndsWith(".pptx"))
            {
                return true;
            }
            return false;
        }

        private bool exist(string fileName,int agendaID)
        {
            FileDAO fileDao = Factory.getInstance<FileDAO>();
            Dictionary<string, object> wherelist = new System.Collections.Generic.Dictionary<string, object>();

            wherelist.Add("fileName", fileName);
            wherelist.Add("agendaID", agendaID);
            var file = fileDao.getOne<FileVO>(wherelist);
            if (file == null)
            {
                return false;
            }
            return true;
        }

        public Status upFile(HttpFileCollectionBase files,int agendaID)
        {
            if (files == null || files.Count <= 0)
            {
                return Status.FAILURE;
            }

            string saveDir = GetSaveDir();

            Status status;

            for (int i = 0; i < files.Count; i++)
            {
                string fileName = files[i].FileName;

                if (!checkFileFormat(fileName))
                {
                    return Status.FILE_NOT_SUPPORT;
                }

                string fileExtension = System.IO.Path.GetExtension(fileName);

                string saveFileName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff")
                    + fileExtension;

                string completeFileName = saveDir + saveFileName;

                files[i].SaveAs(completeFileName);

                FileInfo fi = new FileInfo(completeFileName);

                //插入数据库
                if ((status = addFile(agendaID, fileName, fi.Length, saveFileName)) != Status.SUCCESS)
                {
                    return status;
                }
            }

            return Status.SUCCESS;
        }

        public Status addFile(int agendaID, string fileName, long fileSize, string saveFileName)
        {
            int filesize = (int)((double)fileSize / 1024 + 0.5); //long转为int

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(agendaID);
            if (agendaVo == null)
            {
                Log.DebugInfo("议程不存在");

                return Status.FAILURE;
            }
            //初始化会议操作
            meeting_initOperator(agendaVo.meetingID);

            bool isUpdate = false;
            //判断会议是否开启，如果开启，更新“会议更新状态”，设置数据更新状态
            if (meeting_isOpening())
            {
                meeting_updatefile();//这里file与document指代同一种事物
                isUpdate = true;
            }
            else if (meeting_isOpended())//会议已结束,不允许添加
            {
                return Status.FAILURE;
            }

            FileDAO fileDao = Factory.getInstance<FileDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            //获取当前文件的个数
            wherelist.Add("agendaID", agendaID);
            List<FileVO> fileVolist = fileDao.getAll<FileVO>(wherelist);

            //获取新文件的ID
            int fileID = FileDAO.getID();

            //设置新的文件编号
            int fileIndex = fileVolist == null ? 1 : fileVolist.Count + 1;

            //添加文件
            FileVO fileVo = new FileVO
            {
                fileID = fileID,
                fileName = fileName,
                fileIndex = fileIndex,
                fileSize = filesize,
                filePath = saveFileName,
                agendaID = agendaID,
                isUpdate = isUpdate //判断是否属于会议中新加入的信息
            };

            Log.DebugInfo("待插入的文件路径:"+saveFileName);
            int num = fileDao.insert<FileVO>(fileVo);
            if ( num < 0)
            {
                Log.DebugInfo("文件数据插入失败");
                return Status.FAILURE;
            }
           
            return Status.SUCCESS;
        }

        /// <summary>
        /// 文件转换
        /// </summary>
        public Status convertFile(int agendaID)
        {
            FileDAO fileDao = Factory.getInstance<FileDAO>();
            Dictionary<string, object> wherelist = new System.Collections.Generic.Dictionary<string, object>();

            wherelist.Add("agendaID", agendaID);

            var files = fileDao.getAll<FileVO>(wherelist);

            try
            {
                if (files != null)
                {
                    string saveDir = GetSaveDir();
                    string htmlDir = GetHtmlDir();
                    string htmlRelativeDir = GetHtmlRelativeDir();

                    foreach (var file in files)
                    {
                        string fileName = file.filePath;
                        string sourcePath = saveDir + fileName;

                        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
                        string targetDir = htmlDir + fileNameWithoutExtension + separator;
                        string targetPath = targetDir + fileNameWithoutExtension + ".html";

                        FileInfo fi = new System.IO.FileInfo(targetPath);
                        if (fi.Exists)
                        {
                            continue;
                        }

                        if (!Directory.Exists(targetDir))
                        {
                            Directory.CreateDirectory(targetDir);
                        }

                        string targetRelativeDirectory = htmlRelativeDir + fileNameWithoutExtension + separator;

                        if (!FileConvert.run(sourcePath, targetPath, targetRelativeDirectory))
                        {
                            fileDao.delete(file.fileID);
                            throw new Exception("文件转换失败");
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Log.LogInfo("文件转换失败", e); 
                return Status.FILE_CONVERT_FAIL;
            }

            return Status.SUCCESS;
        }

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
            if (fileVolist == null)
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

        public Status deleteMultipe(List<int> documents)
        {
            if (documents == null || documents.Count == 0)
            {
                return Status.ARGUMENT_ERROR;
            }

            //出错后恢复数据
            var backup = new List<FileVO>();
            Status status = Status.SUCCESS;

            FileDAO fileDao = Factory.getInstance<FileDAO>();
            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            Dictionary<string, object> wherelist = new Dictionary<string, object>();

            foreach (int documentID in documents)
            {
                //获取附件信息
                FileVO fileVo = fileDao.getOne<FileVO>(documentID);
                if (fileVo == null)
                {
                    status = Status.NONFOUND;
                    break;
                }

                //获取议程信息
                AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(fileVo.agendaID);
                if (agendaVo == null)
                {
                    continue;
                }
                //初始化会议操作
                meeting_initOperator(agendaVo.meetingID);
                //判断会议是否 未开启,如果 不是”未开启“，直接退出
                if (!meeting_isNotOpen())
                {
                    return Status.FAILURE;
                }
                //更新其他附件的序号信息
                if (fileDao.updateIndex(fileVo.agendaID, fileVo.fileIndex) < 0)
                {
                    status = Status.FAILURE;
                    break;
                }

                backup.Add(fileVo);

                //删除当前附件
                if (fileDao.delete(fileVo.fileID) < 0)
                {
                    status = Status.FAILURE;
                    break;
                }
            }
            if (status != Status.SUCCESS)
            {
                foreach (var fileVo in backup)
                {
                    fileDao.insert<FileVO>(fileVo);
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
            fileDao.delete(wherelist);//删除数据库中附件对应的记录
            return Status.SUCCESS;
        }

        public string getOriginFileName(string filePath)
        {
            Dictionary<string, object> wherelist = new Dictionary<string, object>();
            FileDAO fileDao = Factory.getInstance<FileDAO>();
            wherelist.Add("filePath", filePath);
            var fileVo = fileDao.getOne<FileVO>(wherelist);
            if (fileVo != null)
            {
                return fileVo.fileName;
            }
            return "";
        }
    }
}