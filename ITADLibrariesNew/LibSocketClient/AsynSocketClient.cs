using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using Asiacell.ITADLibraries_v1.LibLogger;


namespace Asiacell.ITADLibraries_v1.LibSocketClient
{
    public abstract class AsynSocketClient
    {
        private LoggerEntities logger = null;

        private ManualResetEvent ConnectionDone = new ManualResetEvent(false);
        private IPEndPoint destination = null;
        private Socket client = null;

        private string ServerIP = string.Empty;
        private int Port = 0;
        public readonly int Cur_ID = 0;

        public ManageResultData ResultData = null;
 
        public AsynSocketClient(String ServerIP, int Port, LoggerEntities logger)
        {
            this.logger = logger;
            this.ServerIP = ServerIP;
            this.Port = Port;
            ResultData = new ManageResultData();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenConnection()
        {
            ConnectionDone.Reset();
            logger.AddtoLog("","Initializing connection ........", LoggerLevel.Info);
            destination = new IPEndPoint(IPAddress.Parse(ServerIP), Port);
            client = new Socket(destination.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.BeginConnect(destination, new AsyncCallback(OnConnected), client);
            ConnectionDone.WaitOne();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnected(IAsyncResult ar)
        {
            Socket socket = null;
            string CommandID = SocketCommand.GenCommandID;
            try
            {
                logger.AddtoLog(CommandID, "The connection is establishing to : IP :" + destination.Address.ToString() + ", Port : " + destination.Port, LoggerLevel.Info);
                StateObject state = new StateObject();

                socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);

                state.ClientSocket = socket;
                socket.BeginReceive(state.buffer, 0, state.BufferSize, SocketFlags.None, new AsyncCallback(OnReceiver), state);

                ///Add scoket to Pool    
            }
            catch (SocketException ex)
            {
                logger.AddtoLog(CommandID , "Socket has been closed, Error Code :" + ex.ErrorCode + ", Desc:" + ex.Message, LoggerLevel.Error);
                ResultData.AddErrorResult(ServerCommandConstant.SendCommand_Failed, CommandID , "Socket has been closed, Error Code :" + ex.ErrorCode + ", Desc:" + ex.Message, "Login");

            }
            catch (ObjectDisposedException)
            {
                logger.AddtoLog("","Socket has been closed", LoggerLevel.Error);
                
            }
            catch (Exception ex)
            {
                logger.AddtoLog("","Error while initializing connection : " + ex.Message, LoggerLevel.Error);
               
            }
            ConnectionDone.Set();
        }

        //public Socket GetConnection { get { return client; } }

        /// <summary>
        /// Add Error message to result when error occured
        /// </summary>
        /// <param name="messsage"></param>
        private void AddErrorToResult(int ErrorCode,string messsage)
        {
            foreach (string command_id in ResultData.GetAllCommands().Keys)
            {
                ResultData.AddErrorResult(ErrorCode , command_id, messsage, ServerCommandConstant.CommandSubmited);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns> 0 is success,   </returns>
        public int Send(string CommandID, byte[] data)
        {
            string command = Encoding.UTF8.GetString(data);
            string CommandType = command.Substring(0, ServerCommandConstant.ResultType_Len);
            
            logger.AddtoLog("","Submiting command : ID : " + CommandID + ", Command :" + command, LoggerLevel.Info);

            ResultData.AddtoCommand(CommandID, command);
            SocketError IsError = SocketError.HostDown;
            try
            {
                client.BeginSend(data, 0, data.Length, 0, out IsError
                    , new AsyncCallback(OnSender), CommandID);
                if ((int)IsError > 0)
                {
                    ResultData.AddErrorResult(ServerCommandConstant.SendCommand_Failed, CommandID, SocketErroDesc.ErrorDescription(IsError), CommandType);
                }
            }
            catch (SocketException ex)
            {
                ResultData.AddErrorResult(ServerCommandConstant.SendCommand_Failed, CommandID, SocketErroDesc.ErrorDescription((SocketError)ex.ErrorCode), CommandType);
            }
            catch (ObjectDisposedException ex)
            {
                ResultData.AddErrorResult(ServerCommandConstant.SendCommand_Failed, CommandID, ex.Message, CommandType);
            }
            return (int)IsError;
        }

        public int Send(byte[] data)
        {
            SocketError IsError;
            client.BeginSend(data, 0, data.Length, 0, out IsError
                , new AsyncCallback(OnSender), null);
            return (int)IsError;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Send(string data)
        {
            return Send(Encoding.UTF8.GetBytes(data));
        }

        public int Send(string CommandID, string data)
        {
            //CommandProperty 
            return Send(CommandID, Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ir"></param>
        private void OnSender(IAsyncResult ir)
        {
            try
            {
                int bytes = client.EndSend(ir);                
            }
            catch (SocketException ex)
            {
                logger.AddtoLog("","Socket has been closed, Error Code :" + ex.ErrorCode + ", Desc:" + ex.Message, LoggerLevel.Error);
            }
            catch (ObjectDisposedException)
            {
                logger.AddtoLog("","Socket has been closed", LoggerLevel.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void OnReceiver(IAsyncResult ar)
        {
            Socket socket = null;
            StateObject state = null;
            SocketError error = SocketError.Success;

            try
            {
                state = (StateObject)ar.AsyncState;
                socket = state.ClientSocket;
                int byteReader = socket.EndReceive(ar);
                string data = string.Empty;

                if (byteReader > 0)
                {
                    //state.SetDataReciever(byteReader);
                    ResultData.AddResult(Encoding.ASCII.GetString(state.buffer, 0, byteReader));
                    //Reader(0, ref ResultData);
                }
                else  ///Simply if byteReader is 0 the connection is lose
                {
                    logger.AddtoLog("","The connection lose", LoggerLevel.Error);
                    
                    AddErrorToResult((int)error, "The connection lose");

                    return;
                }
                socket.BeginReceive(state.buffer, 0, state.BufferSize, SocketFlags.None, out error, new AsyncCallback(OnReceiver), state);
            }
            catch (SocketException ex)
            {
                logger.AddtoLog("", "Socket has been closed, Error Code :" + ex.ErrorCode + ", Desc:" + ex.Message, LoggerLevel.Error);
                AddErrorToResult((int)error,"Socket has been closed, Error Code :" + ex.ErrorCode + ", Desc:" + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                AddErrorToResult((int)error,"Socket has been closed");
                logger.AddtoLog("","Socket has been closed", LoggerLevel.Error);
            }
        }

        public abstract void OnError(int error, string Message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="IsClearData"></param>
        //public abstract void Reader(int error, ref ManageResultData ResultData);

        /// <summary>
        /// s.Poll returns true if
        /// connection is closed, reset, terminated or pending (meaning no active connection)
        /// connection is active and there is data available for reading
        /// s.Available returns number of bytes available for reading
        /// if both are true:there is no data available to read so connection is not active
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool IsConnected()
        {
            bool part1 = client.Poll(10000, SelectMode.SelectError);
            bool part2 = (client.Available == 0);
            if (part1 & part2)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Close()
        {
            try
            {
                if (client != null)
                {
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
            catch (SocketException ex)
            {
                logger.AddtoLog("", "Socket has been closed, Error Code :" + ex.ErrorCode + ", Desc:" + ex.Message, LoggerLevel.Error);
            }
            catch (ObjectDisposedException)
            {
                logger.AddtoLog("","Socket has been closed", LoggerLevel.Error);
            }
            catch (Exception ex)
            {
                logger.AddtoLog("","Error while initializing connection : " + ex.Message, LoggerLevel.Error);
            }
        }

    }
}
