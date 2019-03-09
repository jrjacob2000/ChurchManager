using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
 
    public class Messages
    {
        public string ErrorMessage
        {
            get;
            set;
        }

        public string SuccessMessage
        {
            get;
            set;
        }

        public static Messages GetModel(string ErrorMessage, string SuccessMessage)
        {
            Messages model = new Messages();
            model.ErrorMessage = ErrorMessage;
            model.SuccessMessage = SuccessMessage;
            return model;
        }
    }
}