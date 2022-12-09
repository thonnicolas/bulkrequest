using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibWCF_v1.Business
{
    public class GSMCommand
    {
        public int GsmCommandId { get; set; }
        public string GsmCommand { get; set; }
        public int SystemCommandId { get; set; }
        public int VersionId { get; set; }
        public int SubscriberType { get; set; }
        public int Sequence { get; set; }
        public string Description { get; set; }
    }
}
