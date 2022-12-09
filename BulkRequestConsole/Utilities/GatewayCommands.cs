using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BulkRequestConsole.Utilities
{
    public class GatewayCommands
    {
        public static readonly string ISCE_PAYASYOUGO = "ISCE_PAYASYOUGO";
        public static readonly string ISCE_PREPAID_SNACKPACKAGE = "ISCE_PREPAID_SNACKPACKAGE";
        public static readonly string ISCE_SNACKPACKAGEPOSTPAID = "ISCE_SNACKPACKAGEPOSTPAID";
        public static readonly string ISCE_REDIRECT_MENU = "ISCE_REDIRECT_MENU";
        public static readonly string ISCE_PREPAID_SPEEDO = "ISCE_PREPAID_SPEEDO";
        public static readonly string ISCE_SPEEDOPOSTPAID = "ISCE_SPEEDOPOSTPAID";
        
        public static readonly string REFILL = "REFILL"; // recharge prepaid

        public static readonly string BSCS_PAY_THROUGH_SC = "BSCS_PAY_THROUGH_SC"; // recharge postpaid
        public static readonly string GETBALANDDATE = "GETBALANDDATE";
        public static readonly string GETACCOUNTDETAILS = "GETACCOUNTDETAILS";

        public static readonly string ISCE_PREPAID_SPEEDO_UNSUBSCRIBE = "ISCE_PREPAID_SPEEDO_UNSUBSCRIBE";

        public static readonly string ISCE_SPEEDOPOSTPAID_UNSUBSCRIBE = "ISCE_SPEEDOPOSTPAID_UNSUBSCRIBE";

        public static readonly string BSCS_GET_USAGE_INFO = "BSCS_GET_USAGE_INFO";
    }
}