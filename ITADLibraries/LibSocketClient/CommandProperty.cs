using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibSocketClient
{
    public class CommandProperty
    {
        private string lResult = string.Empty;
        private DateTime Submited_Date = DateTime.Now;
        private DateTime Responded_Date;
        private bool Responed = false;
        public int ResultCode = -1;

        public string CommandID { set; get; }
        public string Command { set; get; }
        public string Result { set { lResult = value; Responed = true; Responded_Date = DateTime.Now; } get { return lResult; } }

        public bool IsRespond { get { return Responed; } }
        public DateTime Submited_On { get { return Submited_Date; } }
        public DateTime Responded_On { get { return Responded_Date; } }
    }
}
