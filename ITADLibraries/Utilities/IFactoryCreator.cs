using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibDatabase;


namespace Asiacell.ITADLibraries.Utilities
{
    public interface IFactoryCreator
    {        
        IConnectionControllerPool Create(BlockingCollection<object> idleworker);
    }
}
