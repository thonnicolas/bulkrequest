using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BulkRequestConsole.Utilities
{
    public class AppEnum
    {
        public enum SubscriberStatus
        {
            Active = 1,
            Inactive = 0
        }

         
        public enum BundleAction
        {
            Subscribe = 1,
            Unsubscribe = 0
        }

        public enum RegisterStatus
        {
            Registerd = 1,
            None = 0
        }
        
        public enum SubscriberType
        {
            Prepaid = 1,
            Postpaid = 2
        }
        public enum ActionStatus
        {
            Fail = 0,
            Success = 1
        }

        public enum LanguageType
        {
            Arabic = 1,
            Kurdish = 2,
            English = 3
        }

        
    }
}