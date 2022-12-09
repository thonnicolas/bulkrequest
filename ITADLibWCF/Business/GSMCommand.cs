using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibWCF.Business
{
    public class GSMCommand
    {
        public int GsmCommandId { get; set; }
        public string GsmCommand { get; set; }
        public int SystemCommandId { get; set; }
        public int VersionId { get; set; }
        public int SubscriberType { get; set; }
        public int Sequence { get; set; }
        public int GsmCommandTypeId { get; set; }
        public string Description { get; set; }
        //g.url_service_name,g.url_header,g.param1,g.param2, g.param3, g.param4, g.param5
        public string url_service_name { get; set; }
        public string url_header { get; set; }
        public string param1 { get; set; }
        public string param2 { get; set; }
        public string param3 { get; set; }
        public string param4 { get; set; }
        public string param5 { get; set; }
    }

    public class GSMCommandType
    { 
        public const int SOAP=1;
        public const int XML_RPC=2;
        public const int MML=3;
        public const int SQL=4;
        public const int NATIVE_CSharp = 5;

    }
}
