using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.Utilities;
using BulkRequestConsole.Classes;

namespace BulkRequestConsole.Classes
{
    public interface IBulkRequesterFactory 
    {        
        IBulkReqester Create();
    }   
}
