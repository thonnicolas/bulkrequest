using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries_v1.Utilities;
using Asiacell.ITADLibraries_v1.LibLogger;
using System.Collections.Concurrent;
using Asiacell.ITADLibraries_v1.LibDatabase;

namespace Asiacell.ITADLibraries_v1.LibSocket
{
    public class Server
    {
        private LoggerEntities logger = null;

        /// <summary>
        /// To store all of comman request from client by reciever mode interface and acknowleged
        /// </summary>
        private DataStoreCommand clientCommand = null;

        public ConcurrentQueue<String> tranLog = null;

        /// <summary>
        /// To store all of connected client socket
        /// </summary>
        private SocketClientInfo clients = null;

        private SocketClientInfo elementClient = null;

        /// <summary>
        /// Socket Server Listen port
        /// </summary>
        private SocketServer ListenSocket = null;

        public Server(DBConnectionPool db, int ServerPort, int ElementServerPort)
        {
            tranLog = new ConcurrentQueue<string>();
            logger = new LoggerEntities(tranLog);
            clients = new SocketClientInfo();
            clientCommand = new DataStoreCommand(clients, db, logger);
            ListenSocket = new SocketServer(ServerPort, clients, clientCommand, logger, db);            
        }

        public bool Start()
        {
            return ListenSocket.StartSocketListener();
        }        

        public void Stop()
        {
            if (ListenSocket != null) ListenSocket.Close();
            if (logger != null) logger.Dispose();
        }
    }
}
