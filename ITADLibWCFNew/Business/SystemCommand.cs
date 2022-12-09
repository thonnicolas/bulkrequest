using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibWCF_v1.Business
{
    public class SystemCommand
    {
        public int SystemCommandId { get; set; }
        public string CommandName { get; set; }
        public int ElementTypeId { get; set; }
        public string CommandParam { get; set; }
        public string InternalParam { get; set; }
    }
}
