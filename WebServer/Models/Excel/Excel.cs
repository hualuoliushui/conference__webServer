using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using DAL.Base;
using System.Data.OleDb;
using System.Data;
using WebServer.App_Start;
using System.Text;
using System.Reflection;

namespace WebServer.Models.Excel
{
    public class Excel
    {
        public static String XlsConnString = "provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes;IMEX=1';";
        public static String XlsxConnString = "provider=Microsoft.ACE.OleDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR=Yes;IMEX=1';";

        /// <summary>
        /// 设置值类。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="row"></param>
        private void setT<T>(ref T t, DataRow row)
        {
            int index = 0;
            foreach (PropertyInfo p in t.GetType().GetProperties())
            {
                string typeName = p.PropertyType.Name;
                if (string.Compare(typeName, "String") == 0)
                {
                    p.SetValue(t, row[index].ToString());
                }
                else if (string.Compare(typeName, "Int32") == 0)
                {
                    p.SetValue(t, Int32.Parse(row[index].ToString()));
                }
                else if (string.Compare(typeName, "DateTime") == 0)
                {
                    p.SetValue(t, DateTime.Parse(row[index].ToString()));
                }

                index++;
            }
        }
        /// <summary>
        /// 将excel表格中的数据，返回为List<typeparam name="T"></typeparam>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="excelFilePath"></param>
        /// <returns></returns>
        public Status import<T>(String excelFilePath,String tableName,out List<T> tlist) 
            where T : new()
        {
            tlist = new List<T>();

            //获取文件扩展名
            string fileExtension = System.IO.Path.GetExtension(excelFilePath).ToString().ToLower();

            string connString;

            if (string.Compare(fileExtension, ".xlsx") == 0)
            {
                connString = String.Format(XlsxConnString, excelFilePath);
            }
            else if (string.Compare(fileExtension, ".xls") == 0)
            {
                connString = String.Format(XlsConnString, excelFilePath);
            }
            else
            {
                return Status.FILE_NOT_SUPPORT;//只支持xlsx、xls文件
            }

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                using (OleDbCommand cmd = conn.CreateCommand())
                {
                    //获取excel文件中的表格名称列表
                    List<String> tableNames = new List<string>();
                    DataTable tb = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    foreach (DataRow row in tb.Rows)
                    {
                        tableNames.Add(row["TABLE_NAME"].ToString());
                    }
                    if (!tableNames.Contains(tableName + "$"))//表格名称:后加上$
                    {
                        //如果文件不包含该表格；
                        return Status.NONFOUND;
                    }
                    cmd.CommandText = "select * from [" + tableName + "$]";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds, tableName);
                    StringBuilder information = new StringBuilder();
                    DataTable dt = ds.Tables[0];
                    if (dt == null || dt.Rows.Count == 0 )
                    {
                        return Status.NONFOUND;
                    }
                    foreach (DataRow dr in dt.Rows)
                    {
                        T t = new T();
                        setT<T>(ref t, dr);
                        tlist.Add(t);
                    }
                }
            }
            return Status.SUCCESS;
        }
    }
}