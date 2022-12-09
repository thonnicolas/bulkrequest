using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibSocket
{
    public class SocketERRORCode
    {

        public const int Successful = 0;

        #region Login
        public const int InvalidUserPassword = 1;
        public const int User_Already_Login = 2;
        #endregion


        #region Commands
        public const int UnknowCommand = 100;
        public const int Server_Cannot_Get_Requested_command = 101;
        public const int The_Client_Queue_Reached_ToMax = 102;
        public const int The_Command_ID_Already_exists = 103;
        public const int InvalidCommand = 104;
        public const int Internal_Error = 105;
        public const int Incorrect_Parameter = 106;
        public const int DatabaseConnection_Error = 107;
        #endregion



    }
}
