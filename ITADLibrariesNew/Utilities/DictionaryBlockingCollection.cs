using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Asiacell.ITADLibraries_v1.Utilities
{
    public class DictionaryBlockingCollection<Tkey, Tvalue>
    {
        private int Capacity = 0;
        private ConcurrentDictionary<Tkey, Tvalue> collection = null;

        public DictionaryBlockingCollection(int Capacity)
        {
            this.Capacity = Math.Max(1, Capacity);
            collection = new ConcurrentDictionary<Tkey, Tvalue>();
        }


        public bool TryAdd(Tkey key, Tvalue value)
        {
            lock (collection)
            {
                WaitIsFull();
                bool IsAdded = collection.TryAdd(key, value);
                Monitor.Pulse(collection);
                return IsAdded;
            }
        }

        public void Add(Tkey key,Tvalue value)
        {
            lock (collection)
            {
                WaitIsFull();
                collection[key] = value;
                Monitor.Pulse(collection);
            }
        }

        public bool TryRemove(Tkey key, out Tvalue value)
        {
            lock (collection)
            {
                WaitIsEmpty();
                bool IsRemoved = collection.TryRemove(key,out value);
                Monitor.Pulse(collection);
                return IsRemoved;
            }
        }

        public int Count { get { return collection.Count; } }

        public ICollection<Tkey> GetKeys ()
        {
            lock (collection)
            {
                return collection.Keys;            
            }
        }

        private void WaitIsFull()
        {
            lock (collection)
            {
                if (Capacity == 0) return;
                if (IsFull()) Monitor.Wait(collection);
            }
        }

        private void WaitIsEmpty()
        {
            lock (collection)
            {
                if (IsEmpty()) Monitor.Wait(collection);
            }
        }
        
        private bool IsEmpty()
        {
            return collection.Count == 0;
        }

        private bool IsFull()
        {
            return collection.Count == Capacity;
        }
    }
}
