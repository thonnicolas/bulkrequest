using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries_v1.LibLogger;
using Asiacell.ITADLibraries_v1.Utilities;
using Asiacell.ITADLibraries_v1.LibDatabase;


namespace Asiacell.ITADLibraries_v1.Utilities
{
    public interface IFactoryCreator
    {        
        IConnectionControllerPool Create(BlockingCollection<object> idleworker);
    }
}
