using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibSocketClient
{
    sealed class ConstantValue
    {
        #region Command Constant
        
        public const string Prefix_Receiver = "R";
        public const string Prefix_Sender = "S";
        public const string EndCommand = "<%EOC%>"; //End of Command
        public const string EndExit = "<%EXIT%>"; //Close connection
        public const string InquiryRequestCommand = "<%INQUIRY-LINK%>";
        public const string InquiryRespondCommand = "<%INQUIRY-RESPOND%>";

        public const string EnterUserID = "User ID";
        public const string EnterPassword = "Password";
        public const string EnterBindingMode = "Please enter binding mode";
        public const string EnterCommand = "Please enter command";

        public const string LogingSucceed = "Login successful";

        public const string LoginCommand = "LOGIN:";
        public const string RespondCommand = "RESPOND:";
        public const string ResultCommand = "RESULT:";
        public const string AcknowlegeCommand = "ACK:";

        #endregion

        #region Command Lenght constant        
        public const int useridLen = 30; //Max lenght
        public const int passwrodLen = 30; //Max lenght
        public const int bindingmodeLen = 4; //Max lenght
        public const int Key_Len = 32;

        public const int resultcod_len = 8;

        #endregion

        #region Result Code

        public const int Succeed = 0;
        public const int LoginSucceed_BindReceiverMode = 1;
        public const int LoginSucceed_BindSenderMode = 2;
        public const int InvalidUserPassword = 3;
        public const int InvalidBindMode = 4;
        public const int UserAlreadyBind = 5;
        #endregion
    }
}
