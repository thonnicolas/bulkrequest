using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Asiacell.ITADLibraries_v1.LibLogger;

namespace Asiacell.ITADLibraries_v1.LibSocket
{
    public sealed class StateObject
    {
        // Client Receiver socket.
        public Socket ClientSocket = null;

        public readonly DateTime Connected_On = DateTime.Now;

        public String ClientIP = "";

        // Loging flage
        public bool IsLoginSucceed = false;

        // Request for user log
        private bool RequestLog = false;

        // Size of Concurent Session
        public int ConcrrentSession = 100;
        // Size of receive buffer.
        public int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[1024];

        // User Name
        public string UserName = "";
        // Password
        public string Password = "";
        // Binding Mode
        //public string BindingMode = "";

        public Task SendTask = null;

        // Received data string.
        public StringBuilder ReceiverBuffer = new StringBuilder();
        ///

        public SocketBufferData bufferData = null;

        // Command Result (Use only in Sender Mode)
        private BlockingCollection<CommandProperties> DataStoreResult = null;

        /// <summary>
        /// Use when RequestLog = true
        /// </summary>
        public ConcurrentBag<string> UserLog = null;

        public StateObject(DataStoreCommand store, LoggerEntities logger)
        {
            bufferData = new SocketBufferData(this, store, logger);
            DataStoreResult = new BlockingCollection<CommandProperties>();
        }

        public bool EnterToRespondResult(CommandProperties Result)
        {
            if (Result != null)
                Result.Completed_Time = DateTime.Now;
            DataStoreResult.Add(Result);            
            return true;
        }


        public CommandProperties GetResponResult()
        {
            CommandProperties comm = DataStoreResult.Take();
            bufferData.RemoveCommand(comm.CommandID);
            return comm;
        }

        public void GetResponResult(out CommandProperties Result)
        {
            DataStoreResult.TryTake(out Result);
            bufferData.RemoveCommand(Result.CommandID);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool SetLog
        {
            get { return RequestLog; }
            set
            {
                if (value && UserLog == null) UserLog = new ConcurrentBag<string>();
                RequestLog = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void SetToLog(string message)
        {
            if (RequestLog)
                UserLog.Add(message);
        }       

    }
}
