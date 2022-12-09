using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries_v1.LibSocket
{
    public class SocketCommandConstant
    {
        #region Lenght
        public const int Login_Len = 79;
        public const int Logout_Len = 49;
        
        public const int ProcessID_Len = 32;

        public const int ErrorCode_Len = 8;
        public const int ResultCode_Len = 8;
        public const int UserName_Len = 15;
        public const int Password_Len = 15;
        public const int ResultType_Len = 9;

        //part of element 
        public const int CommandHeader_Len = 8;
        public const int CommandID_Len = 32;
        public const int CommandRequest_Len = 50;
        #endregion


        #region Constant
        public const string SeperaterParam = ",";
        #endregion

        #region Command
        public const string EndCommand = "<%EOC%>"; //End of Command
        public const string InquiryRequestCommand = "<%INQUIRY-LINK%>"; //Enquiry Link
        public const string InquiryRespondCommand = "<%INQUIRY-RESPOND%>"; //Enquiry Report
        #endregion

        #region Command Header
        /// <summary>
        /// Login command header
        /// </summary>
        public const string LoginCommand = "ITMDW-LGI:";
        /// <summary>
        /// Result command header
        /// </summary>
        public const string ResultCommand = "ITMDW-RES:";
        /// <summary>
        /// Acknowlege command header
        /// </summary>
        public const string AcknowlegeCommand = "ITMDW-ACKL:";
        /// <summary>
        /// Execute Command Header
        /// </summary>
        public const string CommandSubmited = "ITMDW-CMD:";
        /// <summary>
        /// Logout Command header
        /// </summary>
        public const string LogoutCommand = "ITMDW-LGO:";
        /// <summary>
        /// Response Command header for Element
        /// </summary>
        public const string ElemRepCommand = "ELM-REP:";
        /// <summary>
        /// Resquest Command header for Element
        /// </summary>
        public const string ElemReqCommand = "ELM-REQ:";

        #endregion
    }
}
