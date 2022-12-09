using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Asiacell.ITADLibraries_v1.LibServerSocket
{
    class GenerateSocketID
    {
        private static long socketid = 0;
        //public GenerateSocketID(int IndexStart)
        //{
        //    socketid = IndexStart;
        //}
        public static long GetSocketID { get { return Interlocked.Increment(ref socketid); } }
    }
}
