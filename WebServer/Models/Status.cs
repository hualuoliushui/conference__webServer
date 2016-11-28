using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models
{
    /// <summary>
    /// 表示返回结果的状态码，及描述
    /// </summary>
    public enum Status
    {
        FAILURE=-1,
        SUCCESS=0,
        FORMAT_ERROR=1,
        DATABASE_OPERATOR_ERROR=2,
        DATABASE_CONTENT_ERROR=3,
        NONFOUND=4,
        ARGUMENT_ERROR=5,
        PERMISSION_DENIED=6,
        FILE_NOT_SUPPORT=7,
        FILE_PATH_ERROR=8
    }

    public class Message
    {
        public static Dictionary<int, string> msgs;
        static Message()
        {
            msgs = new Dictionary<int, string>();
            msgs.Add((int)Status.SUCCESS, "操作成功");
            msgs.Add((int)Status.FAILURE, "操作失败");
            msgs.Add((int)Status.ARGUMENT_ERROR, "参数不为空");
            msgs.Add((int)Status.FORMAT_ERROR, "参数格式出错");
            msgs.Add((int)Status.PERMISSION_DENIED, "无权限");
            msgs.Add((int)Status.NONFOUND, "无数据");
            msgs.Add((int)Status.DATABASE_CONTENT_ERROR, "数据库内容出错");
            msgs.Add((int)Status.DATABASE_OPERATOR_ERROR, "数据库操作出错");
            msgs.Add((int)Status.FILE_NOT_SUPPORT, "文件格式不支持上传");
            msgs.Add((int)Status.FILE_PATH_ERROR, "文件路径或文件不存在");
        }
    }

    public class System_Info
    {
        public static Dictionary<int, string> info;
        static System_Info()
        {
            info = new Dictionary<int, string>();
           
        }
    }
}