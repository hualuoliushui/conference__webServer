using Aspose.Cells;
using Aspose.Words;
using Aspose.Slides;
using Aspose.Pdf;
using WebServer.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;

namespace WebServer.Models.Document.FileConvertService
{
    public class AsposeMethod : FileConvertBase
    {
      
        public override bool Word2HTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            try
            {
                //如果已存在，则重新生成
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }

              
                    Aspose.Words.Document doc = new Aspose.Words.Document(sourcePath);
                    doc.Save(targetPath, Aspose.Words.SaveFormat.Html);

                //Log.DebugInfo("修改html文件中的src元素的路径");
                changeSrc(targetPath, targetRelativeDirectory);

                return true;
            }
            catch (Exception e)
            {
                Log.LogInfo("aspose转换word文档", e);

                return false;
            }
        }

        public override bool Excel2HTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            try
            {
                //如果已存在，则重新生成
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }

                Aspose.Cells.Workbook excel = new Workbook(sourcePath);
                excel.Save(targetPath, Aspose.Cells.SaveFormat.Html);

                //修改html文件中的src元素的路径
                changeHref(targetPath, targetRelativeDirectory);

                return true;
            }
            catch (Exception e)
            {
                Log.LogInfo("aspose转换excel文档", e);
                return false;
            }
        }

        private string getTargetDirectory(string targetPath)
        {
            int lastPosition = targetPath.LastIndexOf('/');
            return targetPath.Substring(0, lastPosition + 1);
        }

        public override bool PPT2HTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            try
            {
                //如果已存在，则重新生成
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }

                Aspose.Slides.Presentation ppt = new Presentation(sourcePath);
                if (ppt == null)
                {
                    throw new Exception("ppt文件无效或者ppt文件被加密！");
                }

                ppt.Save(targetPath, Aspose.Slides.Export.SaveFormat.Html);
                return true;
            }
            catch (Exception e)
            {
                Log.LogInfo("aspose转换ppt文档", e);
                return false;
            }
        }
    }
}