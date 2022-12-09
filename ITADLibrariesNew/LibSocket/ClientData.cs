using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries_v1.LibSocket
{
    public class ClientData<Tkey, Tvalue> : IDictionary<Tkey, Tvalue>
    {
        private object locker = new object();

        private Dictionary<Tkey, Tvalue> data;

        BlockingCollection<Tvalue> tt = new BlockingCollection<Tvalue>();


        public ClientData()
        {
            data = new Dictionary<Tkey, Tvalue>();
        }

        public void Add(Tkey key, Tvalue value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(Tkey key)
        {
            throw new NotImplementedException();
        }

        public ICollection<Tkey> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(Tkey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(Tkey key, out Tvalue value)
        {
            throw new NotImplementedException();
        }

        public ICollection<Tvalue> Values
        {
            get { throw new NotImplementedException(); }
        }

        public Tvalue this[Tkey key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Add(KeyValuePair<Tkey, Tvalue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<Tkey, Tvalue> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<Tkey, Tvalue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<Tkey, Tvalue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<Tkey, Tvalue>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
