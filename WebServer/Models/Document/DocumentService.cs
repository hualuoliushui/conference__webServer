using System.Collections.Generic;

using DAL.DAO;
using DAL.DAOVO;
using DAL.DAOFactory;
using System.IO;
using System.Configuration;
using System;
using System.Text;
using WebServer.Models.Document.FileConvertService;
using WebServer.App_Start;

namespace WebServer.Models.Document
{
    public class DocumentService : Organizor
    {
        public Status addFile(string userName, int agendaID, string fileName, long fileSize, string fileFullPath)
        {
            int filesize = (int)fileSize;//long转为int

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(agendaID);
            if (agendaVo == null)
            {
                return Status.FAILURE;
            }
            //初始化会议操作
            meeting_initOperator(agendaVo.meetingID);
            //验证拥有者权限
            //if (!meeting_validatePermission(userName))
            //{
            //    return Status.PERMISSION_DENIED;
            //}

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
                filePath = fileFullPath,
                agendaID = agendaID,
                isUpdate = isUpdate //判断是否属于会议中新加入的信息
            };

            if (fileDao.insert<FileVO>(fileVo) != 1)
            {
                return Status.FAILURE;
            }
            else
            {
                return Status.SUCCESS;
            }
        }

        /// <summary>
        /// 文件转换
        /// </summary>
        /// <param name="srcFileAbsolutePath"></param>
        /// <param name="targetFileAbsolutePath"></param>
        /// <param name="targetFileRelativeDirectory_Root"></param>
        public Status convertFile(string srcFileAbsolutePath,string targetFileAbsolutePath,string targetFileRelativeDirectory_Root)
        {
            #region 检查文件信息
            //检查源文件是否存在
            FileInfo srcFI = new FileInfo(srcFileAbsolutePath);
            if (!srcFI.Exists)
            {//如果源文件不存在，则直接返回
                return Status.NONFOUND;
            }

            //检查目标文件是否存在
            FileInfo targetFI = new FileInfo(targetFileAbsolutePath);
            if (targetFI.Exists)
            {//如果存在，就删除该文件
                targetFI.Delete();
            } 
            #endregion

            //获取文件扩展名，带.
            string fileExtension = System.IO.Path.GetExtension(srcFileAbsolutePath);
            //初始化文件转换对象
            FileConvertBase method = new OfficeMethod();
            try
            {
                switch (fileExtension)
                {
                   //转换word
                    case ".doc":
                    case ".docx":
                        if (!method.WordToHTML(srcFileAbsolutePath, targetFileAbsolutePath, targetFileRelativeDirectory_Root))
                        {//文件转换失败
                            Log.DebugInfo("word文件转换失败");
                            return Status.FILE_CONVERT_FAIL;

                        }
                        break;

                    //转换excel
                    case ".xls":
                    case ".xlsx":
                        if (!method.ExcelToHTML(srcFileAbsolutePath, targetFileAbsolutePath, targetFileRelativeDirectory_Root))
                        {//文件转换失败
                            Log.DebugInfo("excel文件转换失败");
                            return Status.FILE_CONVERT_FAIL;
                        }
                        break;

                    //转换ppt
                    case ".ppt":
                    case ".pptx":
                         if (!method.PPToHTML(srcFileAbsolutePath, targetFileAbsolutePath, targetFileRelativeDirectory_Root))
                        {//文件转换失败
                            Log.DebugInfo("ppt文件转换失败");
                            return Status.FILE_CONVERT_FAIL;
                        }
                        break;

                    default://其他文件不支持
                        return Status.FILE_NOT_SUPPORT;
                }
            }
            catch (Exception e)
            {
                Log.LogInfo("文件转换异常", e);
                return Status.FILE_CONVERT_EXCEPTION;
            }
            return Status.SUCCESS;
        }

        //上传文件到服务器端的路径
        public string getSrcFileAbsoluteDirectory(int agendaID,out string fileRelativeDirectory_Origin)
        {
            fileRelativeDirectory_Origin = "";

            AgendaDAO agendaDao = Factory.getInstance<AgendaDAO>();
            AgendaVO agendaVo = agendaDao.getOne<AgendaVO>(agendaID);
            if (agendaVo == null)//议程不存在，文件路径设置为空
            {
                return null;
            }
            int meetingID = agendaVo.meetingID;
            //上传后文件存储在服务器端的路径
            //获取存储文件的根目录
            string WebServerRootPath = System.Web.HttpContext.Current.Server.MapPath("\\");
           
            //设置相对origin目录的目录路径名
            fileRelativeDirectory_Origin =  @"\"+meetingID + @"\" + agendaID + @"\";

            //源文件相对指定根目录的目录
            string fileRelativeDirectory_Root = @"\upfiles\origin" + fileRelativeDirectory_Origin;

            string srcFileAbsoluteDirectory = WebServerRootPath + fileRelativeDirectory_Root.Substring(1,fileRelativeDirectory_Root.Length-1);
            if (!Directory.Exists(srcFileAbsoluteDirectory))//不存在该目录就创建
            {
                Directory.CreateDirectory(srcFileAbsoluteDirectory);
            }
            return srcFileAbsoluteDirectory;
        }

        /// <summary>
        /// 返回目标文件的绝对路径名和相对于rootPath目录的目录名(一直到目标文件)
        /// </summary>
        /// <param name="srcFileAbsolutePath">源文件绝对路径名</param>
        /// <param name="srcFileRelativeDiretory_Origin">相对于origin目录的目录名（一直到源文件)</param>
        /// <param name="targetFileRelativeDirectory_Root">相对于rootPath目录的目录名(一直到目标文件)</param>
        /// <returns></returns>
        public string getTargetFileAbsolutePath(string srcFileAbsolutePath,string srcFileRelativeDiretory_Origin,out string targetFileRelativeDirectory_Root){
            targetFileRelativeDirectory_Root = "";
            //获取目标文件所在目录的根目录,(为apiserver网站的根目录）
            string APIServerRootPath = ConfigurationManager.AppSettings["APIServerRootPath"];
            //获取无扩展名的文件名。
            string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(srcFileAbsolutePath);
            //获取文件扩展名,带"."
            string srcFileExtension = System.IO.Path.GetExtension(srcFileAbsolutePath);
            //获取去除.的文件扩展名
            string srcFileExtensionWithDot = srcFileExtension.Substring(1, srcFileExtension.Length - 1);
            //构造目标文件名
            string targetFileName = fileNameWithoutExtension + "_" + srcFileExtensionWithDot; 

            //设置，目标文件相对指定root目录的目录路径名，out返回
            targetFileRelativeDirectory_Root = @"\upfiles\html" + srcFileRelativeDiretory_Origin + targetFileName + @"\";
          
            #region 构造目标文件绝对路径名
            StringBuilder targetFileAbsolutePath = new StringBuilder();
            //添加指定网站的根目录
            targetFileAbsolutePath.Append(APIServerRootPath);
            //添加目标文件相对指定root目录的目录路径名
            targetFileAbsolutePath.Append(targetFileRelativeDirectory_Root);
            //检查目标文件目录的绝对路径是否存在
            if (!Directory.Exists(targetFileAbsolutePath.ToString()))
            {//不存在，就创建
                Directory.CreateDirectory(targetFileAbsolutePath.ToString());
            }
            //添加目标文件名、"."、扩展名=(默认html)
            targetFileAbsolutePath.Append(targetFileName);
            targetFileAbsolutePath.Append(".html");
            #endregion

            //返回目标文件绝对路径名
            return targetFileAbsolutePath.ToString();
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
                //初始化会议操作
                meeting_initOperator(agendaVo.meetingID);
                //验证权限
                if (meeting_validatePermission(userName))//有权限就删除
                {
                    //判断会议是否 未开启,如果 不是”未开启“，直接退出
                    if (!meeting_isNotOpen())
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
            List<FileVO> fileVolist = fileDao.getAll<FileVO>(wherelist);
            fileDao.delete(wherelist);//删除数据库中附件对应的记录
            if (fileVolist != null)
            {   //删除议程下所有附件
                foreach (FileVO fileVo in fileVolist)
                {
                    deleteFile(fileVo.filePath);
                }
            }
            return Status.SUCCESS;
        }

        /// <summary>
        /// 删除指定路径的文件？？
        /// </summary>
        /// <param name="srcFileAbsoluteDirectory"></param>
        /// <returns></returns>
        private static Status deleteFile(string filePath)
        {
            //删除该路径的文件
            FileInfo fi = new FileInfo(filePath);
            fi.Delete();
            return Status.SUCCESS;
        }

        //文件的上传和下载
    }
}