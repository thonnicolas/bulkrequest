using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.Utilities;

namespace Asiacell.ITADLibraries.LibSocket
{
    public class CommandProperties
    {

        public readonly string PID = string.Empty;
        public readonly string UserName;
        //Commad ID, Hexa 8dig
        public readonly string CommandID;
        public readonly string Command;
        private string _CommandName = string.Empty;
        private string[] _CommandParamValue = new string[0];
        private string _MSISDN = string.Empty;
        private string _IMSI = string.Empty;            

        public string Result { get; set; }                
        public readonly DateTime Requested_time;
        public DateTime Completed_Time { get; set; }
        public int ErroCode { set; get; }

        public int ElementID { get; set; }
        public int SystemCommandID { get; set; }
             

        public CommandProperties(string UserName, string CommandID, string Command)
        {
            this.UserName = UserName;
            this.CommandID = CommandID;
            this.Command = Command;
            Requested_time = DateTime.Now;
            PID = Functions.GetPID;
            SetCommand(Command);
        }

        /// <summary>
        /// Get System Command Name
        /// </summary>
        public string GetCommandName { get { return _CommandName; } }
        /// <summary>
        /// Get MSISDN
        /// </summary>
        public string GetMSISDN { get { return _MSISDN; } }
        /// <summary>
        /// Get IMSID
        /// </summary>
        public string GetIMSI { get { return _IMSI; } }

        public List<string> GetValue { get { return _CommandParamValue.ToList<string>(); } }

        /// <summary>
        /// Splite Client command and set to different argurments
        /// </summary>
        /// <param name="Command"></param>
        private void SetCommand(string Command)
        {
            try
            {
                string[] getClientCommand = Functions.SpliteBySplace(Command);
                if (getClientCommand != null && getClientCommand.Length > 1)
                {
                    _CommandName = getClientCommand[0].ToUpper();
                    _CommandParamValue = new string[getClientCommand.Length - 1];

                    

                    if (Functions.IsMSISDN(getClientCommand[1]))
                        _MSISDN = getClientCommand[1];
                    else if (Functions.IsIMSI(getClientCommand[1]))
                        _IMSI = getClientCommand[1];
                   
                    Array.Copy(getClientCommand, 1, _CommandParamValue,0, _CommandParamValue.Length);                     
                }
                else
                {
                    _CommandName = Command;
                }
            }
            catch { }
        }

        
    }
}
