using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibWCF.Business
{

    public class FilterSensitive
    {
        
        
        public string CommandName { get; set; }
        public string SensitiveParams { get; set; }
    }


    public class CommandOutputFilter
    {
        public int CommandFilterId { get; set; }
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public int SystemCommandId { get; set; }
        public string Pattern { get; set; }
        public string ApplyOn { get; set; }
    }

    public class SystemCommand
    {
        public int SystemCommandId { get; set; }
        public string CommandName { get; set; }
        public int ElementTypeId { get; set; }
        public string CommandParam { get; set; }
        public string InternalParam { get; set; }
        public int IsIsdn { get; set; }
        public int IsImsi { get; set; }
    }

    public class Endpoint
    {
        public int EndpointId { get; set; }
        public string EndpointUrl { get; set; }
        public int ElementTypeId { get; set; }
        public int Status { get; set; }
        public string Type { get; set; } 
    }
}
