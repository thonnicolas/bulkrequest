using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibLogger
{
    //<ProcessID:XXXXXXXXXXXXXXXXXX><ERRORCODE><Client-IP><UserName><CommandID:Command><Request_On - Respond_On><Result>
    public class LoggerAttribute
    {
        public readonly String ModuleName = string.Empty;
        public readonly String ProcessID = string.Empty;
        public readonly int ERRORCODE = 0;
        public readonly String Client_IP = string.Empty;
        public readonly String UserName = string.Empty;
        public readonly String CommandID = string.Empty;
        public readonly String Command = string.Empty;
        public readonly DateTime Sumited_On = DateTime.Now;
        public readonly DateTime Responded_On = DateTime.Now;
        public readonly String Result = string.Empty;
        public readonly LoggerLevel loglevl;
        public readonly bool IsOnlyResult = false;

        public LoggerAttribute(String ModuleName, String ProcessID, int ERRORCODE, String ClientIP, String UserName, String CommandID,
            String Command, DateTime Submited_On, DateTime Responded_On, String Result, LoggerLevel loglevel)
        {
            this.ProcessID = ProcessID;
            this.ERRORCODE = ERRORCODE;
            this.Client_IP = ClientIP;
            this.UserName = UserName;
            this.CommandID = CommandID;
            this.Command = Command;
            this.Sumited_On = Submited_On;
            this.Responded_On = Responded_On;
            this.Result = Result;
            this.loglevl = loglevel;
            this.ModuleName = ModuleName;
            this.IsOnlyResult = false;
        }

        public LoggerAttribute(String ModuleName, String Result, LoggerLevel loglevel)
        {
            this.Result = Result;
            this.loglevl = loglevel;
            this.ModuleName = ModuleName;
            this.IsOnlyResult = true;

            this.Sumited_On = this.Responded_On = DateTime.Now;
        }

        public LoggerAttribute(String PID, String ModuleName, String Result, LoggerLevel loglevel)
        {
            this.Result = Result;
            this.loglevl = loglevel;
            this.ModuleName = ModuleName;
            this.ProcessID = PID;
            this.IsOnlyResult = true;
            this.Sumited_On = this.Responded_On = DateTime.Now;
        }

        public LoggerAttribute(String PID, String ModuleName, String Result, DateTime Submited_On, DateTime Responded_On, LoggerLevel loglevel)
        {
            this.Result = Result;
            this.loglevl = loglevel;
            this.ModuleName = ModuleName;
            this.ProcessID = PID;
            this.IsOnlyResult = true;

            this.Sumited_On = Submited_On;
            this.Responded_On = Responded_On;
        }
    }
}
