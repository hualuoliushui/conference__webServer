using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebServer.Models.Tools
{
    public class ModelStateHelper
    {
        public static List<string> errorMessages(ModelStateDictionary ModelState)
        {
            List<string> errorMessages = new List<string>();
            foreach (string key in ModelState.Keys)
            {
                
                ModelState value = ModelState[key];
                foreach (ModelError modelError in value.Errors)
                    errorMessages.Add(modelError.ErrorMessage); 
            }

            return errorMessages;
        }
    }
}