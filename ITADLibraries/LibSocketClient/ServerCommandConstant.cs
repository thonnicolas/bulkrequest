using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibSocketClient
{
    class ServerCommandConstant
    {
        #region Lenght
        public const int Login_Len = 79;
        public const int Logout_Len = 25;
        public const int CommandID_Len = 32;
        public const int ResultCode_Len = 8;
        public const int UserName_Len = 15;
        public const int Password_Len = 15;
        public const int ResultType_Len = 9;
        #endregion

        #region Command
        public const string EndCommand = "<%EOC%>"; //End of Command
        public const string InquiryRequestCommand = "<%INQUIRY-LINK%>";
        public const string InquiryRespondCommand = "<%INQUIRY-RESPOND%>";
        #endregion

        #region Command Header
        /// <summary>
        /// Login command header
        /// </summary>
        public const string LoginCommand = "ITMDW-LGI";
        /// <summary>
        /// Result command header
        /// </summary>
        public const string ResultCommand = "ITMDW-RES";
        /// <summary>
        /// Acknowlege command header
        /// </summary>
        public const string AcknowlegeCommand = "ITMDW-ACKL";
        /// <summary>
        /// Execute Command Header
        /// </summary>
        public const string CommandSubmited = "ITMDW-CMD";
        /// <summary>
        /// Logout Command header
        /// </summary>
        public const string LogoutCommand = "ITMDW-LGO";
        #endregion

        public const int Successful = 0;

        #region Login
        public const int InvalidUserPassword = 1;
        public const int User_Already_Login = 2;
        #endregion


        #region Server Error code
        public const int UnknowCommand = 100;
        public const int Server_Cannot_Get_Requested_command = 101;
        public const int The_Client_Queue_Reached_ToMax = 102;
        public const int The_Command_ID_Already_exists = 103;        
        #endregion
        #region Client Error code
        public const int SendCommand_Failed = 900;
        public const int LoginError = 901;
        #endregion
    }
}
