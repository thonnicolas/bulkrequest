using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibLogger
{
    public class LoggerDBAttributeTest
    {
        private DateTime _transaction_on = DateTime.Now;
        private int _global_result = -1;
        private int _errorcode = -1;
        private decimal _elementtypeid = -1;
        public string serviceid { get; set; }
        public string userid { get; set; }
        public string clientip { get; set; }
        public string itmdw_ip { get; set; }
        public string itmdw_port { get; set; }
        public DateTime transaction_on { get { return _transaction_on; } set { _transaction_on = value; } }
        public string msisdn_imsi { get; set; }
        public int commandid { get; set; }
        public string command_name { get; set; }
        public string sent_command { get; set; }
        public string command_result { get; set; }
        public string gsm_result { get; set; }
        public int global_result { get { return _global_result; } set { if (value == null) { _global_result = -1; } else { _global_result = value; } } }
        public int errorcode { get { return _errorcode; } set { if (value == null) { _errorcode = -1; } else { _errorcode = value; } } }
        public long request_seq { get; set; }
        public decimal elementtypeid { get { return _elementtypeid; } set { if (value == null) { _elementtypeid = -1; } else { _elementtypeid = value; } } }
        public string comments { get; set; }
        public string param1 { get; set; }
        public string param2 { get; set; }
        public string param3 { get; set; }
        public string param4 { get; set; }
        public string param5 { get; set; }
        public string param6 { get; set; }
        public string param7 { get; set; }
        public string param8 { get; set; }
        public string param9 { get; set; }
        public int flag { get; set; }
    }
}
