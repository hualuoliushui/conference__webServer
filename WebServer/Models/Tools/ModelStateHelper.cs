using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebServer.Models.Tools
{
    public class ModelStateHelper
    {
        public static Dictionary<string, List<string>> errorMessages(ModelStateDictionary ModelState)
        {
            Dictionary<string, List<string>> ret = new Dictionary<string, List<string>>();
            foreach (string key in ModelState.Keys)
            {
                List<string> errorMessages = new List<string>();
                ModelState value = ModelState[key];
                foreach (ModelError modelError in value.Errors)
                    errorMessages.Add(modelError.ErrorMessage);

                if (errorMessages.Count > 0)
                    ret[key] = errorMessages;
            }

            return ret;
        }
    }
}