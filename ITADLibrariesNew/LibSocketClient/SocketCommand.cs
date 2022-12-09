using System;
using System.Text;
using Asiacell.ITADLibraries_v1.Utilities;

namespace Asiacell.ITADLibraries_v1.LibSocketClient
{
    class SocketCommand
    {

        /// <summary>
        /// Generate Login Command
        /// ITMDW-LGI:{1}{2}{3}<%EOC%>
        /// {1} : CommandID , in hexa 8bytes
        /// {2} : charactor, 15 bytes
        /// {3} : charactor, 15 bytes
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <param name="BindMode"></param>
        /// <returns></returns>
        public static byte[] GenLoginCommand(string UserName, string Password, string CommandID)
        {
            string logMessage = ServerCommandConstant.LoginCommand + ":" + CommandID +
                            RightPad(UserName, ' ', ServerCommandConstant.UserName_Len) +
                            RightPad(Password, ' ', ServerCommandConstant.Password_Len);
            return ConvertToByteWithEnd(logMessage);
        }

        /// <summary>
        /// ITMDW-CMD:{1}{2}<%EOC%>
        /// {1}:CommandID, Hexa 8 bytes
        /// {2}:Command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static byte[] GenerateCommand(string command, string CommandID)
        {            
            string commandstr = ServerCommandConstant.CommandSubmited + ":" + CommandID + command;
            return ConvertToByteWithEnd(commandstr);
        }

        public static byte[] GenerateLogout(string CommandID)
        {
            string commandstr = ServerCommandConstant.LogoutCommand + ":" + CommandID;
            return ConvertToByteWithEnd(commandstr);
        }


        //private static string ToHexa(int value)
        //{
        //    return Convert.ToString(value, 16);
        //}

        //private static int HexaToInt(string value)
        //{
        //    return Convert.ToInt32(value, 16);
        //}

        public static string GenCommandID
        {
            //return LefPad(ToHexa(CommandID), '0', ServerCommandConstant.CommandID_Len);
            get { return LefPad(Functions.GetPID, ' ', ServerCommandConstant.CommandID_Len); }
                 
        }


        /// <summary>
        /// Convert string to byte
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static byte[] ConvertToByteWithEnd(string command)
        {
            command = command + ServerCommandConstant.EndCommand;
            return Encoding.ASCII.GetBytes(command);
        }

        private static byte[] ConvertToByte(string command)
        {
            return Encoding.ASCII.GetBytes(command);
        }

        private static string ConvertToString(byte[] command)
        {
            return Encoding.ASCII.GetString(command);
        }



        /// <summary>
        /// ITMDW-RES:{1}{2}{3}{4}{5}<%EOC%>
        /// {1}:ErrorCode, 8bytes integer
        /// {2}:CommandID, hexa 8bytes
        /// {3}:ResultType, 9 bytes
        /// {4}:username, 15bytes
        /// {5}:Result , all the raise before end command
        /// </summary>
        /// <param name="Respond"></param>
        /// <returns></returns>
        public static CommandRespondProperties IsResponcommand(string data)
        {

            CommandRespondProperties respon = null;

            if (!String.IsNullOrWhiteSpace(data) && data.IndexOf(ServerCommandConstant.ResultCommand) == -1)
            {
                return respon;
            }

            try
            {
                int resultcode_position = ServerCommandConstant.ResultCommand.Length+ 1;
                int commandid_position = resultcode_position + ServerCommandConstant.ResultCode_Len;
                int resultType_Position = commandid_position + ServerCommandConstant.CommandID_Len;
                int UserName_Position = resultType_Position + ServerCommandConstant.ResultType_Len;                
                int result_Position = UserName_Position + ServerCommandConstant.UserName_Len;

                int resultcode = Convert.ToInt32(data.Substring(resultcode_position, ServerCommandConstant.ResultCode_Len));
                //int command_id = HexaToInt(data.Substring(commandid_position, ServerCommandConstant.CommandID_Len));
                string command_id = data.Substring(commandid_position, ServerCommandConstant.CommandID_Len);
                string resultType = data.Substring(resultType_Position, ServerCommandConstant.ResultType_Len);
                string username = data.Substring(UserName_Position, ServerCommandConstant.UserName_Len).Trim();
                string result = data.Substring(result_Position);
                
                respon = new CommandRespondProperties(resultcode, command_id, result,resultType);
            }
            catch
            {

            }

            return respon;
        }



        /// <summary>
        /// Right to left Padding with "PadString"
        /// </summary>
        /// <param name="value">Data</param>
        /// <param name="PadChar">padding character</param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static string LefPad(string value, char PadChar, int precision)
        {
            string newValue = value;
            if (newValue.Length >= precision)
                newValue = newValue.Substring(0, precision);
            else
                newValue = newValue.PadLeft(precision, PadChar);

            return newValue;
        }

        /// <summary>
        /// Left to Right Padding character
        /// </summary>
        /// <param name="value"></param>
        /// <param name="PadChar"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        private static string RightPad(string value, char PadChar, int precision)
        {
            string newValue = value;
            if (newValue.Length >= precision)
                newValue = newValue.Substring(0, precision);
            else
                newValue = newValue.PadRight(precision, PadChar);

            return newValue;
        }


    }
}
