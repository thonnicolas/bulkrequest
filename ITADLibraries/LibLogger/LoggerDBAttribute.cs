using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.LibLogger
{
    [Serializable()]
    public class LoggerDBAttribute
    {
        private int _elementtypeid = -1;
        private int _error_code = -1;
        private DateTime _log_date = DateTime.Now;
        public DateTime log_date { get { return _log_date; } set { _log_date = value; } }
        public string transaction_id {get; set;}
        public DateTime request_date {get; set;}
        public DateTime respond_date {get; set;}
        public string msisdn {get; set;}
        public string imsi {get; set;}
        public string server_node_name {get; set;}
        public string command_name {get; set;}
        public int command_id {get; set;}
        public int elementtypeid { get { return _elementtypeid; } set { if (value == null) { _elementtypeid = -1; } else { _elementtypeid = value; } } }
        public int error_code { get { return _error_code; } set { if (value == null) { _error_code = -1; } else { _error_code = value; } } }
        public string description {get; set;}
        public string param1 {get; set;}
        public string param2 {get; set;}
        public string param3 {get; set;}
        public string param4 {get; set;}
        public string param5 {get; set;}
        public string param6 {get; set;}
        public string param7 {get; set;}
        public string param8 {get; set;}
        public string param9 {get; set;}
        //public string result { get; set; }
        //public string command { get; set; }
        public string username { get; set; }
        public string client_ip { get; set; }
        public string element_node_name { get; set; }
        public string external_data { get; set; }
        public DateTime element_start { get; set; }
        public DateTime element_end { get; set; }
        public LoggerDBAttribute Clone()
        {
            return (LoggerDBAttribute)(this.MemberwiseClone());
        }
        
    }
}
