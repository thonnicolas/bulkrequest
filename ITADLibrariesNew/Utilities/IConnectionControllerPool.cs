using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Asiacell.ITADLibraries_v1.Utilities
{
    public interface IConnectionControllerPool
    {
         object RunWork(object command);
         void Process(object command, ExecuteCallDelegate callDelegate);
         void Process(object command);
         void Dispose();
         //bool IsAlive { get; }
         //Thread GetThreadWorker { get; }
    }
}
