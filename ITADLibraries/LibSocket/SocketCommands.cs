using System;
using System.Text;
using Asiacell.ITADLibraries.Utilities;

namespace Asiacell.ITADLibraries.LibSocket
{
    public class SocketCommands
    {

        public SocketCommands()
        {

        }

        /// <summary>
        /// Check if login command will return below value
        /// ITMDW-LGI:{1}{2}{3}<%EOC%>
        /// {1} : CommandID , 32 by
        /// {2} : charactor, 15 bytes
        /// {3} : charactor, 15 bytes
        /// </summary>
        /// <param name="LoginString"></param>
        /// <param name="CommandID"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static bool LoginCommand(string LoginString, ref string CommandID, ref string UserName, ref string Password)
        {
            bool Islogin = false;
            if (string.IsNullOrWhiteSpace(LoginString)) return Islogin;

            try
            {
                if (LoginString.Length != SocketCommandConstant.Login_Len)
                {
                    CommandID = LoginString.Substring(SocketCommandConstant.LoginCommand.Length, SocketCommandConstant.CommandID_Len);
                }
                else
                {
                    int LoginHeader_Position = 0;
                    int CommandID_Position = SocketCommandConstant.LoginCommand.Length;
                    int UserName_Position = CommandID_Position + SocketCommandConstant.CommandID_Len;
                    int Password_Position = UserName_Position + SocketCommandConstant.UserName_Len;

                    string LoginHead = LoginString.Substring(LoginHeader_Position, SocketCommandConstant.LoginCommand.Length);
                    if (LoginHead.ToUpper() == SocketCommandConstant.LoginCommand)
                    {
                        CommandID = LoginString.Substring(CommandID_Position, SocketCommandConstant.CommandID_Len);
                        UserName = LoginString.Substring(UserName_Position, SocketCommandConstant.UserName_Len).Trim().Trim('\0');
                        Password = LoginString.Substring(Password_Position, SocketCommandConstant.Password_Len).Trim().Trim('\0');

                        Islogin = true;
                    }
                }
            }
            catch { }
            return Islogin;
        }

        /// <summary>
        /// ITMDW-CMD:{1}{2}<%EOC%>
        /// {1}:CommandID : 32 bytes
        /// {2}:Command
        /// </summary>
        /// <param name="CommandString"></param>
        /// <param name="CommandID"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        public static bool IsExecutedCommand(ref string data, ref string CommandID, ref string Command)
        {
            bool IsCommand = false;

            try
            {
                if (data != null && data.Length > 0)
                {
                    //data = data.TrimStart();
                    int CommandHeader_Position = 0;
                    int CommandID_Position = SocketCommandConstant.CommandSubmited.Length;
                    int Command_Position = CommandID_Position + SocketCommandConstant.CommandID_Len;
                    int EndCommand_Position = data.IndexOf(SocketCommandConstant.EndCommand);
                    string b_RemainData = "";
                    string b_CommandHeader = data.Substring(CommandHeader_Position, SocketCommandConstant.CommandSubmited.Length).ToUpper();

                    if (b_CommandHeader == SocketCommandConstant.CommandSubmited)
                    {
                        CommandID = data.Substring(CommandID_Position, SocketCommandConstant.CommandID_Len);
                        Command = data.Substring(Command_Position, EndCommand_Position - Command_Position);
                        b_RemainData = data.Substring(EndCommand_Position + SocketCommandConstant.EndCommand.Length);

                        if (b_RemainData.Length > 0)
                            data = b_RemainData;
                        else
                            data = "";
                        IsCommand = true;
                    }
                    else
                    {
                        CommandID = data.Substring(CommandID_Position, SocketCommandConstant.CommandID_Len);
                    }

                    //string CommandHeader = CommandString.Substring(CommandHeader_Position, ServerCommandConstant.CommandSubmited.Length).ToUpper();
                    //CommandID = CommandString.Substring(CommandID_Position, ServerCommandConstant.CommandID_Len);
                    //Command = CommandString.Substring(Command_Position, (CommandString.Length - ServerCommandConstant.EndCommand.Length - 1)).Trim();


                }
            }
            catch { }
            return IsCommand;
        }


        /// <summary>
        /// ITMDW-RES:{1}{2}{3}{4}{5}{6}<%EOC%>
        /// {1}:ErrorCode, 8bytes integer
        /// {2}:CommandID, 32 bytes
        /// {3}:ResultType, 9 bytes
        /// {4}:username, 15 bytes
        /// {5}:PID, 32 bytes
        /// {6}:Result
        /// </summary>
        /// <param name="ErrorCode"></param>
        /// <param name="CommandID"></param>
        /// <param name="UserName"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static byte[] GenerateResult(string ProcessID, int ErrorCode, string CommandID, string ResultType, string UserName, string Message)
        {
            Message = string.IsNullOrWhiteSpace(Message) ? "" : Message;

            int MaxLenResult = SocketCommandConstant.ResultCommand.Length + SocketCommandConstant.ResultCode_Len
                            + SocketCommandConstant.CommandID_Len + SocketCommandConstant.ResultType_Len
                            + SocketCommandConstant.UserName_Len + Message.Length + SocketCommandConstant.ProcessID_Len;

            byte[] Result = new byte[MaxLenResult];
            string getResult = SocketCommandConstant.ResultCommand +
                                Functions.LefPad(ErrorCode, '0', SocketCommandConstant.ResultCode_Len) +
                                //CommandID +
                                Functions.LefPad(CommandID, ' ', SocketCommandConstant.CommandID_Len) +
                                Functions.RightPad(ResultType, ' ', SocketCommandConstant.ResultType_Len) +
                                Functions.RightPad(UserName, ' ', SocketCommandConstant.UserName_Len) +
                                Functions.LefPad(ProcessID, ' ', SocketCommandConstant.ProcessID_Len) +
                                Message + SocketCommandConstant.EndCommand;

            Result = Encoding.ASCII.GetBytes(getResult);
            return Result;
        }

        /// <summary>
        /// ITMDW-REQ:{1}{2}{3}{4}{5}<%EOC%>
        /// {1}:Header, string 8bytes
        /// {2}:CommandID, 32 Bytes
        /// {3}:UserID, string 15bytes
        /// {4}:Command, string 50bytes
        /// </summary>
        /// <param name="RequestCommand"></param>
        /// <param name="PID"></param>
        /// <param name="User"></param>
        /// <param name="CommandString"></param>
        /// <returns></returns>
        public static bool ElementCommandRequest(string ComamndRequest, ref string CommandID, ref string UserName, ref string CommandString)
        {
            bool IsCommand = false;

            if (string.IsNullOrWhiteSpace(ComamndRequest) || ComamndRequest.Length < SocketCommandConstant.CommandHeader_Len) return IsCommand;

            try
            {
                int CommandHeader_Position = 0;
                int CommandID_Position = CommandHeader_Position + SocketCommandConstant.CommandHeader_Len;
                int UserName_Position = CommandID_Position + SocketCommandConstant.CommandID_Len;
                int CommandRequset = UserName_Position + SocketCommandConstant.UserName_Len;

                string RequestType = ComamndRequest.Substring(CommandHeader_Position, SocketCommandConstant.ElemRepCommand.Length);
                if (RequestType.ToUpper() == SocketCommandConstant.ElemRepCommand)
                {
                    //string RequestType = RequestCommand.Substring(CommandHeader_Position, CommandID_Position);
                    CommandID = ComamndRequest.Substring(CommandID_Position, SocketCommandConstant.CommandID_Len);
                    UserName = ComamndRequest.Substring(UserName_Position, SocketCommandConstant.UserName_Len).Trim().Trim('\0');
                    CommandString = ComamndRequest.Substring(CommandRequset, SocketCommandConstant.CommandRequest_Len).Trim().Trim('\0');
                    IsCommand = true;
                }

            }
            catch (Exception e) {  }

            return IsCommand;
        }

        /// <summary>
        /// ITMDW-RES:{1}{2}{3}{4}{5}<%EOC%>
        /// {1}:Header, string 8bytes
        /// {2}:CommandID, hex 32bytes
        /// {3}:UserID, string 15bytes
        /// {4}:Command Result Unlimite
        /// {5}<%EOC%>  End Of Command
        /// </summary>
        /// <param name="CommandID"></param>
        /// <param name="UserName"></param>
        /// <param name="CommandResult"></param>
        /// <returns></returns>
        public static byte[] GenerateElementResponse(string CommandID, string UserName, string CommandResult)
        {
            string CommandResponse = string.Empty;

            CommandResponse += SocketCommandConstant.ElemRepCommand;
            CommandResponse += Functions.LefPad(CommandID, '0', SocketCommandConstant.CommandID_Len);
            CommandResponse += Functions.RightPad(UserName, ' ', SocketCommandConstant.UserName_Len);
            CommandResponse += CommandResult;
            CommandResponse += SocketCommandConstant.EndCommand;

            return ASCIIEncoding.ASCII.GetBytes(CommandResponse);
        }
        /// <summary>
        /// ITMDW-LGO:{1}<%EOC%>
        /// {1}:CommandID, 32 bytes
        /// </summary>
        /// <param name="LogoutString"></param>
        /// <param name="CommandID"></param>
        /// <returns></returns>
        public static bool LogoutCommand(string LogoutString, ref string CommandID)
        {
            bool Islogout = false;
            if (string.IsNullOrWhiteSpace(LogoutString)) return Islogout;

            try
            {
                if (LogoutString.Length != SocketCommandConstant.Logout_Len)
                {
                    CommandID = LogoutString.Substring(SocketCommandConstant.LogoutCommand.Length, SocketCommandConstant.CommandID_Len);
                }
                else
                {
                    string LogoutHeader = LogoutString.Substring(0, SocketCommandConstant.LogoutCommand.Length);
                    CommandID = LogoutString.Substring(SocketCommandConstant.LogoutCommand.Length, SocketCommandConstant.CommandID_Len);

                    Islogout = true;
                }
            }
            catch { }
            return Islogout;
        }        

        private static string ByteToString(byte[] data)
        {
            return Encoding.ASCII.GetString(data);
        }

        private bool IsRejetectedData(string data)
        {
            return String.IsNullOrWhiteSpace(data);
        }
    }
}
