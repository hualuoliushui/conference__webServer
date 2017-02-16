using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WebServer.Models.Document.FileConvertService
{
    public abstract class FileConvertBase
    {
        /// <summary>
        /// 将word文档转换为html，同时将html文件中的src元素的路径加上targetRelativeDirectory
        /// </summary>
        /// <param name="sourcePath">源文件路径</param>
        /// <param name="targetPath">目标文件路径</param>
        /// <param name="targetRelativeDirectory">目标文件相对目录路径（相对网站根目录）</param>
        /// <returns></returns>
        abstract public bool Word2HTML(string sourcePath, string targetPath, string targetRelativeDirectory);

        abstract public bool Excel2HTML(string sourcePath, string targetPath, string targetRelativeDirectory);

        abstract public bool PPT2HTML(string sourcePath, string targetPath, string targetRelativeDirectory);

        private static System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("utf-8");

        protected static bool changeSrc(string htmlFilePath, string relativeDirectory)
        {
            try
            {
                Encoding originEncoding = FileEncoding.EncodingType.GetType(htmlFilePath);
                string content = File.ReadAllText(htmlFilePath, originEncoding);
                string pattern = @"src=""([^""]*)""";
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
                string resultContent = regex.Replace(content, "src=\"" + relativeDirectory + "$1\"");
                File.WriteAllText(htmlFilePath, resultContent, encoding);
                return true;
            }
            catch (Exception e)
            {
                
                return false;
            }
        }

        protected static bool changeHref(string htmlFilePath, string relativeDirectory)
        {
            try
            {
                string content = File.ReadAllText(htmlFilePath, encoding);
                string patternHref = @"href=""([^""]*)""";
                Regex regexHref = new Regex(patternHref, RegexOptions.IgnoreCase);
                string resultContent = regexHref.Replace(content, "href=\"" + relativeDirectory + "$1\"");

                string patternSrc = @"src=""([^""]*)""";
                Regex regex = new Regex(patternSrc, RegexOptions.IgnoreCase);
                resultContent = regex.Replace(resultContent, "src=\"" + relativeDirectory + "$1\"");
                File.WriteAllText(htmlFilePath, resultContent, encoding);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }


    ///获取文件编码
    /// <summary> 
    /// FileEncoding 的摘要说明 
    /// </summary> 
    namespace FileEncoding
    {
        /// <summary> 
        /// 获取文件的编码格式 
        /// </summary> 
        public class EncodingType
        {
            /// <summary> 
            /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
            /// </summary> 
            /// <param name=“FILE_NAME“>文件路径</param> 
            /// <returns>文件的编码类型</returns> 
            public static System.Text.Encoding GetType(string FILE_NAME)
            {
                FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
                Encoding r = GetType(fs);
                fs.Close();
                return r;
            }

            /// <summary> 
            /// 通过给定的文件流，判断文件的编码类型 
            /// </summary> 
            /// <param name=“fs“>文件流</param> 
            /// <returns>文件的编码类型</returns> 
            public static System.Text.Encoding GetType(FileStream fs)
            {
                byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
                byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
                byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
                Encoding reVal = Encoding.Default;

                BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
                int i;
                int.TryParse(fs.Length.ToString(), out i);
                byte[] ss = r.ReadBytes(i);
                if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
                {
                    reVal = Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                {
                    reVal = Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                {
                    reVal = Encoding.Unicode;
                }
                r.Close();
                return reVal;

            }

            /// <summary> 
            /// 判断是否是不带 BOM 的 UTF8 格式 
            /// </summary> 
            /// <param name=“data“></param> 
            /// <returns></returns> 
            private static bool IsUTF8Bytes(byte[] data)
            {
                int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
                byte curByte; //当前分析的字节. 
                for (int i = 0; i < data.Length; i++)
                {
                    curByte = data[i];
                    if (charByteCounter == 1)
                    {
                        if (curByte >= 0x80)
                        {
                            //判断当前 
                            while (((curByte <<= 1) & 0x80) != 0)
                            {
                                charByteCounter++;
                            }
                            //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                            if (charByteCounter == 1 || charByteCounter > 6)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //若是UTF-8 此时第一位必须为1 
                        if ((curByte & 0xC0) != 0x80)
                        {
                            return false;
                        }
                        charByteCounter--;
                    }
                }
                if (charByteCounter > 1)
                {
                    throw new Exception("非预期的byte格式");
                }
                return true;
            }
        }
    } 
}