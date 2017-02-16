using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebServer.App_Start;

namespace WebServer.Models.Document.FileConvertService
{
    public class FileConvert
    {
        public static bool run(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            string fileExtension = System.IO.Path.GetExtension(sourcePath);

            FileConvertBase method = new 
                AsposeMethod();
            try
            {
                switch (fileExtension)
                {
                    //转换word
                    case ".doc":
                    case ".docx":
                        method = new OfficeMethod();
                        if (!method.Word2HTML(sourcePath, targetPath, targetRelativeDirectory))
                        {//文件转换失败
                            Log.DebugInfo("word文件转换失败");
                            return false;
                        }
                        break;

                    //转换excel
                    case ".xls":
                    case ".xlsx":
                        method = new OfficeMethod();
                        if (!method.Excel2HTML(sourcePath, targetPath, targetRelativeDirectory))
                        {//文件转换失败
                            Log.DebugInfo("excel文件转换失败");
                            return false;
                        }
                        break;

                    //转换ppt
                    case ".ppt":
                    case ".pptx":
                        method = new OfficeMethod();
                        if (!method.PPT2HTML(sourcePath, targetPath, targetRelativeDirectory))
                        {//文件转换失败
                            Log.DebugInfo("ppt文件转换失败");
                            return false;
                        }
                        break;

                    default://其他文件不支持
                        return false;
                }
            }
            catch (Exception e)
            {
                Log.LogInfo("文件转换异常", e);
                return false;
            }
            return true;
        }
    }
}