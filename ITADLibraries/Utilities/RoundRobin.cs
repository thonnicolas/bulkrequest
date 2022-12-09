using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace Asiacell.ITADLibraries.Utilities
{
    public class RoundRobin<T> where T : class
    {
        private int roundID = 0;
        private object locked = new object();

        public T Get(ConcurrentDictionary<int, T> Elements)
        {
            try
            {
                return Elements[GetRoundIndex(Elements)];
            }
            catch (Exception ex)
            { }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int GetRoundIndex(ConcurrentDictionary<int, T> Elements)
        {
            lock (locked)
            {
                int currentId = roundID;
                if (currentId >= Elements.Count)
                {
                    currentId = 0;
                    roundID = 1;
                }
                else
                    Interlocked.Increment(ref roundID);

                return currentId;
            }
        }

    }
}
