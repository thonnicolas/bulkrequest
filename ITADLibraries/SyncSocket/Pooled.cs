using System;
using Asiacell.ITADLibraries.Utilities;
using System.Collections.Generic;
using System.Threading;

namespace Asiacell.ITADLibraries.Sockets
{
    public abstract class Pooled : IDisposable
    {
        object locker = new object();
        private readonly Logger logger;

        /// <summary>The maximum size of the connection pool.</summary>
        private static int POOL_SIZE;

        /// <summary>Queue of available socket connections.</summary>
        private Queue<Object> Pool = new Queue<Object>();

        public Pooled()
        {

        }


        /// <summary>
        /// Get an open SocketClient from the connection pool.
        /// </summary>
        /// <returns>SocketClient returned from the pool or new socket
        /// opened.</returns>
        public Object GetSocket()
        {
            Object obj;
            while (true)
            {
                lock (locker)
                {
                    for (int i = 0; i < this.Pool.Count; i++)
                    {
                        obj = this.Pool.Dequeue();
                        return obj;
                    }
                }

                Thread.Sleep(100);

            }
        }

        public void ReturnSocket(Object socket)
        {
            lock (locker) AddSocket(socket); //this.clientPool[socket.GetSocketID].IsUsed = false;
        }

        public int GetCurrentPool { get { lock (locker) return Pool.Count; } }
        public int GetPoolSize { get { return POOL_SIZE; } }

        /// <summary>
        /// Return the given socket back to the socket pool.
        /// </summary>
        /// <param name="socket">Socket connection to return.</param>
        private void AddSocket(Object socket)
        {
            lock (locker)
            {
                if (this.Pool.Count < Pooled.POOL_SIZE)
                {
                    if (socket != null)
                    {
                        this.Pool.Enqueue(socket);
                    }
                }
            }
        }

        public virtual void Dispose()
        {

        }

    }
}
