using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibLogger
{
    public class LoggerType
    {
        //<ProcessID:XXXXXXXXXXXXXXXXXX><ERRORCODE><Client-IP><UserName><CommandID><Command><Request_On - Respond_On><Result>
        public const int Submit_Command = 1;
        public const int Result = 2;
    }
}
