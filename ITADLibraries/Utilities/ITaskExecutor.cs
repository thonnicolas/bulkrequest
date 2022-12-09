using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.Utilities
{
    public interface ITaskExecutor
    {
        bool Execute(ConcurrentQueue<object> transactionObjects);
    }
}
