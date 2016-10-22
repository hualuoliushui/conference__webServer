using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models.Tools
{
    public class JsonHelper
    {
        public static string GetJsonString(HttpContext context){
             using (var reader = new System.IO.StreamReader(context.Request.InputStream))
            {
                String jsonString = reader.ReadToEnd();
                return jsonString;
            }
        }

        public static TObject GetObject<TObject>(string jsonString) where TObject : class,new()
        {
            TObject tObject = null;
            if (!string.IsNullOrEmpty(jsonString))
            {
                //Json字符串解析
                tObject = JsonConvert.DeserializeObject<TObject>(jsonString);  
            }
            return tObject;
        }

        public static TObject GetObjectService<TObject>(HttpContext context) where TObject : class,new()
        {
            return GetObject<TObject>(GetJsonString(context));
        }  
    }
}