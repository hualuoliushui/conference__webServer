using Microsoft;
using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WebServer.App_Start;

namespace WebServer.Models.Document.FileConvertService
{
    public class OfficeMethod:FileConvertBase
    {
        public override bool Word2HTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {

            Microsoft.Office.Interop.Word.Application docApp = null;
            Microsoft.Office.Interop.Word.Document doc = null;
            object missing = Type.Missing;
            try
            {
                if (!sourcePath.EndsWith(".doc") && !sourcePath.EndsWith(".docx"))
                {
                    return false;
                }


                // 缺省参数 
                object Unknown = Type.Missing;
                object object_True = true;

                //如果已存在，则重新生成
                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                docApp = new Microsoft.Office.Interop.Word.ApplicationClass();
                if (docApp == null)
                {
                    Log.DebugInfo("docApp空指针");
                }
                // 只读方式打开 
                object readOnly = true;
                // 指定另存为格式(html) 
                object formatHtml = Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML;

                object source = sourcePath;
                object target = targetPath;
                // 打开doc文件 
                //Log.DebugInfo("打开doc文件..");
                Microsoft.Office.Interop.Word.Documents docs = docApp.Documents;
                if (docs == null)
                {
                    Log.DebugInfo("docs空指针");
                }
                if (Unknown == null)
                {
                    Log.DebugInfo("Unknown空指针");
                }
                //Log.DebugInfo("source object:"+ source);
                doc = docs.Open(ref source, ref Unknown, ref readOnly,
                    ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown,
                    ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                if (doc == null)
                {
                    Log.DebugInfo("doc空指针");
                }
                //Log.DebugInfo("另存为HTML");
                //关闭拼写检查
                doc.SpellingChecked = false;
                doc.ShowSpellingErrors = false;
                doc.ShowGrammaticalErrors = false;


                Log.DebugInfo("word src Path : " + sourcePath);
                Log.DebugInfo("word target Path : " + targetPath);

                doc.SaveAs(ref target, ref formatHtml,
                      ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown,
                      ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown, ref Unknown);
                //Log.DebugInfo("另存为html,成功");
            }
            catch (Exception e)
            {
                Log.LogInfo("word转换html:", e);
                return false;
            }
            finally
            {
                //Log.DebugInfo("finally部分");
                if (doc != null)
                {
                    doc.Close(true, missing, missing);
                    doc = null;
                }
                if (docApp != null)
                {
                    docApp.Quit();
                    docApp = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                //Log.DebugInfo("word转化结束");
            }

            //Log.DebugInfo("修改html文件中的src元素的路径");
            changeSrc(targetPath, targetRelativeDirectory);
            return true;


            //throw new NotImplementedException();
        }

        public override bool Excel2HTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            Microsoft.Office.Interop.Excel.XlFileFormat targetType = Microsoft.Office.Interop.Excel.XlFileFormat.xlHtml;
            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.ApplicationClass excelApp = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            try
            {
                if (!sourcePath.EndsWith(".xls") && !sourcePath.EndsWith(".xlsx"))
                {
                    return false;
                }

                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                excelApp = new Microsoft.Office.Interop.Excel.ApplicationClass();
                if (excelApp == null)
                {
                    Log.LogInfo("无法创建excel对象，可能您的电脑未安装excel");
                    return false;
                }
                Log.DebugInfo("excel srcPath: " + sourcePath);
                Log.DebugInfo("excel targetPath: " + targetPath);

                object target = targetPath;
                object type = targetType;
                Microsoft.Office.Interop.Excel.Workbooks workbooks = excelApp.Workbooks;


                workbook = workbooks.Open(sourcePath, missing, missing, missing, missing, missing,
                    missing, missing, missing, missing, missing, missing, missing, missing, missing);
                workbook.SaveAs(target, type, missing, missing, missing, missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange, missing, missing, missing, missing, missing);

            }
            catch (Exception e)
            {
                Log.LogInfo("excel转换html:", e);
                return false;
            }
            finally
            {
                if (workbook != null)
                {
                    workbook.Close(false, missing, missing);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    workbook = null;
                    GC.Collect();
                }
                if (excelApp != null)
                {
                    excelApp.Quit();
                    excelApp = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();

                //修改html文件中的src元素的路径
                changeHref(targetPath, targetRelativeDirectory);
            }

            return true;

            //throw new NotImplementedException();
        }

        public override bool PPT2HTML(string sourcePath, string targetPath, string targetRelativeDirectory)
        {
            Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType targetFileType = Microsoft.Office.Interop.PowerPoint.PpSaveAsFileType.ppSaveAsJPG;
            object missing = Type.Missing;
            Microsoft.Office.Interop.PowerPoint.Application pptApp = null;
            Microsoft.Office.Interop.PowerPoint.Presentation presentation = null;
            try
            {
                if (!sourcePath.EndsWith(".ppt") && !sourcePath.EndsWith(".pptx"))
                {
                    return false;
                }

                if (File.Exists(targetPath))
                {
                    File.Delete(targetPath);
                }
                object target = targetPath;
                pptApp = new Microsoft.Office.Interop.PowerPoint.ApplicationClass();
                Microsoft.Office.Interop.PowerPoint.Presentations presentations = pptApp.Presentations;
               
                presentation = presentations.Open(sourcePath, MsoTriState.msoTrue, MsoTriState.msoFalse, MsoTriState.msoFalse);
                presentation.SaveAs(targetPath, targetFileType, Microsoft.Office.Core.MsoTriState.msoTrue);
            }
            catch (Exception e)
            {
                Log.LogInfo("ppt转换html:", e);
                return false;
            }
            finally
            {
                if (presentation != null)
                {
                    presentation.Close();
                    presentation = null;
                }
                if (pptApp != null)
                {
                    pptApp.Quit();
                    pptApp = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            pptHelper(targetPath, targetRelativeDirectory);

            return true;
        }

        /// <summary>
        /// 创建html文件
        /// 将目标目录下的png图片链接到html文件
        /// </summary>
        /// <param name="targetPath"></param>
        /// <param name="targetRelativeDirctory"></param>
        private void pptHelper(String targetPath, String targetRelativeDirctory)
        {
            //获取目录分割符
            string separator = Path.DirectorySeparatorChar.ToString();
            //获取图片当前目录名称
            string pictureCurDir = System.IO.Path.GetFileNameWithoutExtension(targetPath);
            //构造图片目录路径
            int pictureDirctoryLength = targetPath.LastIndexOf(Path.DirectorySeparatorChar);
            string pictureDirectory = targetPath.Substring(0, pictureDirctoryLength) + separator + pictureCurDir;
            //获取所有图片名称
            var fileArray = Directory.GetFiles(pictureDirectory);

            //将图片名称按1，2，3.。。10，11，12.。排序
            List<String> files = new List<string>();
            for (int i = 0; i < fileArray.Length; i++)
            {
                files.Add(System.IO.Path.GetFileName(fileArray[i]));
            }
            Comparison<string> comparison = new Comparison<string>((string x, string y) =>
            {
                //默认图片文件名为“幻灯片i.jpg'
                int a_point_position = x.LastIndexOf('.');
                int b_point_position = y.LastIndexOf('.');
                int a = Int32.Parse(x.Substring(3, a_point_position - 3));
                int b = Int32.Parse(y.Substring(3, b_point_position - 3));
                return a < b ? -1 : (a == b ? 0 : 1);
            });
            files.Sort(comparison);

            StringBuilder htmlContent = new StringBuilder();
            //添加图片链接
            //...
            htmlContent.Append("<!DOCTYPE html>");
            htmlContent.Append("<html>");
            htmlContent.Append("<head>");

            htmlContent.Append("<meta http-equiv=\"Content-Type\" content=\"text/html;charset=utf-8\"/>");

            htmlContent.Append("</head>");
            htmlContent.Append("<body>");
            foreach (var file in files)
            {
                htmlContent.Append("<div>");
                htmlContent.Append("<img ");
                htmlContent.Append(" src=\"");
                //添加网站相对根目录的目录路径
                htmlContent.Append(targetRelativeDirctory);
                htmlContent.Append(separator);
                //添加当前目录名
                htmlContent.Append(pictureCurDir);
                htmlContent.Append(separator);
                //添加当前文件名
                htmlContent.Append(file);
                htmlContent.Append("\"/>");
                htmlContent.Append("</div>");
            }
            htmlContent.Append("</body>");
            htmlContent.Append("</html>");
            using (FileStream fsWrite = new FileStream(targetPath, FileMode.Create))
            {
                //写入文件
                //。。。
                byte[] content = System.Text.Encoding.UTF8.GetBytes(htmlContent.ToString());
                fsWrite.Write(content, 0, content.Length);
                fsWrite.Flush();
            }
        }
    }
}