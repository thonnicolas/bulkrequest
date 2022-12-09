using System;
using System.Collections.Generic;
using Asiacell.ITADLibraries_v1.Utilities;
using System.Threading;

namespace Asiacell.ITADLibraries_v1.Sockets
{
    public class PoolSocket : IDisposable
    {
        object locker = new object();
        private readonly Logger logger;
  
        /// <summary>The maximum size of the connection pool.</summary>
        private static int POOL_SIZE;

        /// <summary>Queue of available socket connections.</summary>
        private Queue<SocketClient> clientPool = new Queue<SocketClient>();

        private static string IP;
        private static int Port;
        private static int sendReceiveTimeout;
        private static int BufferSize;

        /// <summary>
        /// Socket connection pool
        /// </summary>
        /// <param name="IP">Host IP</param>
        /// <param name="Port">Host Port</param>
        /// <param name="sendReceiveTimeout">Send/Receive time out</param>
        /// <param name="BufferSize">Stream buffer size</param>
        /// <param name="POOL_SIZE">Connection Pool size</param>
        public PoolSocket(string IP, int Port, int sendReceiveTimeout, int BufferSize, int POOL_SIZE)
        {
            //Make sure the MAX Pool size at least 1
            PoolSocket.POOL_SIZE = Math.Max(1, POOL_SIZE);

            PoolSocket.IP = IP;
            PoolSocket.Port = Port;
            PoolSocket.sendReceiveTimeout = sendReceiveTimeout;
            PoolSocket.BufferSize = BufferSize;

            logger = new Logger(GetType());

            for (int i = 0; i < PoolSocket.POOL_SIZE; i++)
            {
                lock (locker)
                {
                    SocketClient client = new SocketClient(IP, Port, sendReceiveTimeout, BufferSize);
                    if (client.Open())
                    {
                        logger.Info(client.ReadLine());
                        client.Reset();
                        client.Write("mao:654321ITMDW-STRING-END\r\n");
                        logger.Info(client.ReadLine());
                        AddSocket(client);
                    }
                }
            }

        }


        /// <summary>
        /// Get an open SocketClient from the connection pool.
        /// </summary>
        /// <returns>SocketClient returned from the pool or new socket
        /// opened.</returns>
        public SocketClient GetSocket()
        {
            SocketClient socket;
            while (true)
            {
                lock (locker)
                {
                    for (int i = 0; i < this.clientPool.Count; i++)
                    {
                        socket = this.clientPool.Dequeue();
                        if (!socket.IsUsed)
                        {
                            if (socket.IsAlive)
                            {
                                socket.IsUsed = true;
                                return socket;
                            }
                            else
                            {
                                if (socket.Open())
                                {
                                    socket.IsUsed = true;
                                    return socket;
                                }
                            }
                        }
                    }
                }

                Thread.Sleep(100);

            }
        }

        public void ReturnSocket(SocketClient socket)
        {
            lock (locker) AddSocket(socket); //this.clientPool[socket.GetSocketID].IsUsed = false;
        }

        public int GetCurrentPool { get { lock (locker) return clientPool.Count; } }
        public int GetPoolSize { get { return POOL_SIZE; } }

        /// <summary>
        /// Return the given socket back to the socket pool.
        /// </summary>
        /// <param name="socket">Socket connection to return.</param>
        private void AddSocket(SocketClient socket)
        {
            lock (locker)
            {
                if (this.clientPool.Count < PoolSocket.POOL_SIZE)
                {
                    if (socket != null)
                    {
                        if (socket.IsAlive)
                        {
                            // Set the socket back to blocking and enqueue
                            socket.IsUsed = false;
                            this.clientPool.Enqueue(socket);
                        }
                        else
                        {
                            if (socket.Open())
                            {
                                socket.IsUsed = false;
                                this.clientPool.Enqueue(socket);
                            }
                        }
                    }
                }
                else
                {
                    // Number of sockets is above the pool size, so just
                    // close it.
                    socket.Dispose();
                }
            }
        }

        //Close all connection
        internal void CloseAll()
        {
            for (int i = 0; i < this.clientPool.Count; i++)
            {
                SocketClient sock = this.clientPool.Dequeue();
                sock.Dispose();
            }
        }

        public void Dispose()
        {
            try
            {
                CloseAll();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
