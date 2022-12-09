using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Net;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibraries.LibDatabase;

namespace Asiacell.ITADLibraries.LibSocket
{
    class SocketElementServer
    {
        private readonly LoggerEntities logger;

        //To make sure that in the same time accept only one connection
        private ManualResetEvent AllDone = new ManualResetEvent(false);

        private DBConnectionPool db = null;

        //User info
        private ClientUserID AllUserLogin = new ClientUserID();

        //List of socket info
        private SocketClientInfo SocketInfo = null;

        //Store Requested Command from Client
        private DataStoreCommand store = null;

        //Generate Local IP & Port
        private IPEndPoint localEndPoint = null;

        //Socket Listener Server
        private Socket ListenerServer;

        private volatile bool IsClosed = false;


        private Task socketTask = null;

        /// <summary>
        /// SocketServer Control
        /// </summary>
        /// <param name="Port">Server Port</param>
        /// <param name="SocketInfo"></param>
        /// <param name="store"></param>
        public SocketElementServer(int Port, SocketClientInfo SocketInfo, DataStoreCommand store, LoggerEntities logger, DBConnectionPool db)
        {
            this.logger = logger;
            this.db = db;
            this.localEndPoint = new IPEndPoint(IPAddress.Any, Port);
            this.SocketInfo = SocketInfo;
            this.store = store;
        }

        /// <summary>
        /// Listen Server socket
        /// </summary>
        public bool StartSocketListener()
        {
            bool IsStarted = false;

            try
            {
                //logger.Info("Initializing Socket Server ........");
                logger.AddtoLog("Initializing Socket Server ........", LoggerLevel.Info);
                ListenerServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //Binding Port
                ListenerServer.Bind(localEndPoint);
                ListenerServer.Listen(10);
                //ListenerServer.AcceptAsync(socketMonitor);
                logger.AddtoLog("The Socket Server is listening port " + localEndPoint.Port, LoggerLevel.Info);
                socketTask = Task.Factory.StartNew(delegate
                {
                    while (!(Boolean)IsClosed)
                    {
                        AllDone.Reset();
                        logger.AddtoLog("Waiting for connection ......." + localEndPoint.Port, LoggerLevel.Info);
                        ListenerServer.BeginAccept(new AsyncCallback(OnClientConnect), ListenerServer);
                        //Waiting till get signal from callback "OnClientConnect"
                        AllDone.WaitOne();
                    }
                }, IsClosed, TaskCreationOptions.LongRunning);


                //Check to make sure the Socket port is Listen
                IsStarted = WaitTaskToRun(socketTask);
            }
            catch (SocketException se)
            {
                logger.AddtoLog("Socket has been closed : ErrorCode : " + se.ErrorCode + ", Description : " + se.Message, LoggerLevel.Error);
            }
            catch (Exception ex)
            {
                logger.AddtoLog(ex, LoggerLevel.Error);
            }

            return IsStarted;
        }

        /// <summary>
        /// To check the Socket Listening task with timeout interval
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private bool WaitTaskToRun(Task task)
        {
            DateTime cur_date = DateTime.Now;
            DateTime espected_date = DateTime.Now.AddMinutes(1);

            while (cur_date < espected_date)
            {

                if (task.Status == TaskStatus.Running)
                {
                    break;
                }
                Thread.Sleep(100);
            }

            if (cur_date >= espected_date && task.Status != TaskStatus.Running)
                throw new Exception("The Socket server listening is operation time out");

            return task.Status == TaskStatus.Running;
        }

        /// <summary>
        /// This is the call back function, which will be invoked when a client is connected 
        /// </summary>
        /// <param name="asyn"></param>
        public void OnClientConnect(IAsyncResult asyn)
        {
            Socket GetSocket = null;
            Socket ClientSocket = null;
            string ClientIP = string.Empty;

            try
            {
                // Here we complete/end the BeginAccept() asynchronous call
                // by calling EndAccept() - which returns the reference to
                // a new Socket object
                GetSocket = (Socket)asyn.AsyncState;
                ClientSocket = GetSocket.EndAccept(asyn);

                //Give signal to server can eccept other request
                AllDone.Set();

                ClientIP = ((IPEndPoint)ClientSocket.RemoteEndPoint).Address.ToString();

                logger.AddtoLog("The Client IP (" + ClientIP + ") is establishing connection", LoggerLevel.Info);

                // Send a welcome message to client
                string msg = "Welecome to ITMDW, Please Enter Login Command : ";
                logger.AddtoLog(msg, LoggerLevel.Info);

                // Create the state object.
                StateObject state = new StateObject(store, logger);
                state.ClientSocket = ClientSocket;
                state.ClientIP = ClientIP;
                Send(ClientSocket, msg);
                ClientSocket.BeginReceive(state.buffer, 0, state.BufferSize, 0,
                    new AsyncCallback(LoginCallBack), state);
            }
            catch (ObjectDisposedException)
            {
                logger.AddtoLog("Socket has been closed", LoggerLevel.Error);
            }
            catch (SocketException se)
            {
                logger.AddtoLog("Socket has been closed : ErrorCode : " + se.ErrorCode + ", Description : " + se.Message + ", IP : " + ClientIP, LoggerLevel.Error);
                CloseSocket(ClientSocket);
            }
        }

        /// <summary>
        /// Client Login Validation
        /// </summary>
        /// <param name="AsResult"></param>
        private void LoginCallBack(IAsyncResult AsResult)
        {
            StateObject loginObject = (StateObject)AsResult.AsyncState;
            Socket ClientHandle = loginObject.ClientSocket;

            try
            {
                int bytesReader = ClientHandle.EndReceive(AsResult);

                String content = String.Empty;
                string PID = "";
                if (bytesReader > 0)
                {
                    // There  might be more data, so store the data received so far.
                    loginObject.ReceiverBuffer.Append(Encoding.ASCII.GetString(loginObject.buffer, 0, bytesReader));
                    content = loginObject.ReceiverBuffer.ToString();

                    if (content.IndexOf(SocketCommandConstant.EndCommand) > -1)
                    {
                        loginObject.ReceiverBuffer.Clear();
                        string UserName = string.Empty;
                        string Password = string.Empty;
                        string CommandID = string.Empty;
                        PID = Functions.GetPID;
                        if (SocketCommands.LoginCommand(content, ref CommandID, ref UserName, ref Password))
                        {
                            logger.AddtoLog(PID, "User Login Name : " + UserName, LoggerLevel.Info);
                            logger.AddtoLog(PID, "User Login Password : ***********", LoggerLevel.Info);


                            loginObject.UserName = UserName;
                            loginObject.Password = Password;

                            ClientInfo useinfo = AllUserLogin.GetClientInfo(db.GetConnection(),loginObject.UserName);
                            //if (useinfo != null || useinfo.Password == loginObject.Password)
                            if (useinfo != null && useinfo.Password == loginObject.Password)
                            {
                                if (SocketInfo.GetSocketClient(UserName) != null)
                                {
                                    logger.AddtoLog(PID, "This [" + loginObject.UserName + "] user already login", LoggerLevel.Error);

                                    Send(loginObject.ClientSocket, SocketCommands.GenerateResult(PID,SocketERRORCode.User_Already_Login, CommandID, SocketCommandConstant.LoginCommand, UserName, "This User already login"));

                                    CloseSocket(ClientHandle);
                                    return;
                                }
                                else
                                {
                                    logger.AddtoLog(PID, "Login successful by user :" + loginObject.UserName, LoggerLevel.Info);

                                    Send(loginObject.ClientSocket, SocketCommands.GenerateResult(PID,SocketERRORCode.Successful, CommandID, SocketCommandConstant.LoginCommand, UserName, "Login successful"));

                                    SocketInfo.AddClient(loginObject);
                                    ResponseResult(loginObject, ref loginObject.SendTask);

                                    ClientHandle.BeginReceive(loginObject.buffer, 0, loginObject.BufferSize, SocketFlags.None, new AsyncCallback(ReadCallbackReceiver), loginObject);
                                    return;
                                }
                            }
                            else
                            {
                                logger.AddtoLog(PID, "Incorrect user name or password, please try again", LoggerLevel.Error);

                                Send(loginObject.ClientSocket, SocketCommands.GenerateResult(PID,SocketERRORCode.InvalidUserPassword, CommandID, SocketCommandConstant.LoginCommand, UserName, "Incorrect user name or password, please try again"));
                                CloseSocket(ClientHandle);
                                return;
                            }
                        }
                        else
                        {
                            logger.AddtoLog(PID, "Invalid or Unknow Login Command : " + content, LoggerLevel.Error);
                            Send(loginObject.ClientSocket, SocketCommands.GenerateResult(PID ,SocketERRORCode.UnknowCommand, CommandID, SocketCommandConstant.LoginCommand, UserName, "Invalid or Unknow Login Command"));
                            CloseSocket(ClientHandle);
                            return;
                        }
                    }
                    else
                    {
                        ClientHandle.BeginReceive(loginObject.buffer, 0, loginObject.BufferSize, SocketFlags.None, new AsyncCallback(LoginCallBack), loginObject);
                    }

                }
                else
                {
                    SocketInfo.RemoveClient(loginObject);
                    CloseSocket(ClientHandle);
                    logger.AddtoLog("Socket has been closed" + loginObject.UserName, LoggerLevel.Error);
                }

            }
            catch (SocketException ex)
            {
                logger.AddtoLog("Socket has been closed : ErrorCode : " + ex.ErrorCode + ", Description : " + ex.Message, LoggerLevel.Error);
                CloseSocket(ClientHandle);
                SocketInfo.RemoveClient(loginObject);
            }
            catch (ObjectDisposedException)
            {
                logger.AddtoLog("Socket has been closed", LoggerLevel.Error);
                SocketInfo.RemoveClient(loginObject);
            }
            catch (Exception ex)
            {
                logger.AddtoLog(ex, LoggerLevel.Error);
                CloseSocket(ClientHandle);
                SocketInfo.RemoveClient(loginObject);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void ResponseResult(StateObject state, ref Task SendTask)
        {

            SendTask = Task.Factory.StartNew(delegate
            {
                StateObject senderState = (StateObject)state;
                string PID = string.Empty;
                while (!IsClosed)
                {
                    CommandProperties result = senderState.GetResponResult();
                    if (result != null)
                    {
                        PID = Functions.GetPID;
                        logger.AddtoLog(PID,"", LoggerLevel.Info);
                        Send(senderState.ClientSocket, SocketCommands.GenerateResult(PID,SocketERRORCode.Successful, result.CommandID, SocketCommandConstant.CommandSubmited, state.UserName, result.Result));
                    }
                }
            }, state, TaskCreationOptions.LongRunning);
        }


        /// <summary>
        /// Receiver Call Back
        /// </summary>
        /// <param name="ar"></param>
        private void ReadCallbackReceiver(IAsyncResult ar)
        {
            String content = String.Empty;
            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.ClientSocket;

            try
            {
                // Read data from the client socket. 
                int bytesRead = handler.EndReceive(ar);
                string PID = string.Empty;
                if (bytesRead > 0)
                {

                    state.ReceiverBuffer.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    content = state.ReceiverBuffer.ToString();
                    //
                    if (content.IndexOf(SocketCommandConstant.EndCommand) > -1)
                    {
                        PID = Functions.GetPID;
                        state.ReceiverBuffer.Clear();
                        int LogoutIndex = content.IndexOf(SocketCommandConstant.LogoutCommand);
                        if (LogoutIndex > -1)
                        {
                            
                            logger.AddtoLog(PID,"User <" + state.UserName + "> request to logout", LoggerLevel.Info);
                            //logger.AddtoLog()
                            if (LogoutIndex > 0)
                            {
                                state.bufferData.AddBuffer(content.Substring(0, LogoutIndex - 1));
                                content = content.Substring(LogoutIndex);
                            }
                            string CommandID = string.Empty;
                            if (SocketCommands.LogoutCommand(content, ref CommandID))
                            {
                                Send(state.ClientSocket, SocketCommands.GenerateResult(PID ,SocketERRORCode.Successful, CommandID, SocketCommandConstant.LogoutCommand, state.UserName, "Logout sucess"));
                                SocketInfo.RemoveClient(state);
                                CloseSocket(state.ClientSocket);
                                logger.AddtoLog(PID ,"User <" + state.UserName + "> logout seccessful", LoggerLevel.Info);
                                return;
                            }
                            else
                            {
                                Send(state.ClientSocket, SocketCommands.GenerateResult(PID , SocketERRORCode.UnknowCommand, CommandID, SocketCommandConstant.LogoutCommand, state.UserName, "Ivalid command"));
                            }
                        }
                        else
                            state.bufferData.AddBuffer(content);
                    }

                    handler.BeginReceive(state.buffer, 0, state.BufferSize, 0,
                    new AsyncCallback(ReadCallbackReceiver), state);

                }
                else // if socket return with 0 byte that mean the connection is lost
                {
                    SocketInfo.RemoveClient(state);
                    logger.AddtoLog("The connection is lost", LoggerLevel.Error);
                }

            }
            catch (SocketException ex)
            {
                SocketInfo.RemoveClient(state);
                CloseSocket(handler);

                logger.AddtoLog("Socket has been closed : ErrorCode : " + ex.ErrorCode + ", Description : " + ex.Message, LoggerLevel.Error);
            }
            catch (ObjectDisposedException)
            {
                SocketInfo.RemoveClient(state);
                logger.AddtoLog("Socket has been closed", LoggerLevel.Error);
            }
        }

        /// <summary>
        /// Send to client
        /// </summary>
        /// <param name="state"></param>
        /// <param name="data"></param>
        private void Send(Socket socket, String data)
        {
            // Convert the string data to byte data using ASCII encoding.

            data = data + SocketCommandConstant.EndCommand;

            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            socket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), socket);
        }

        /// <summary>
        /// Send to client
        /// </summary>
        /// <param name="state"></param>
        /// <param name="data"></param>
        private void Send(Socket socket, byte[] data)
        {
            // Begin sending the data to the remote device.
            socket.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(SendCallback), socket);
        }

        /// <summary>
        /// Send Call
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            Socket handler = (Socket)ar.AsyncState;
            try
            {

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);

            }
            catch (SocketException ex)
            {
                logger.AddtoLog("Socket has been closed : ErrorCode : " + ex.ErrorCode + ", Description : " + ex.Message, LoggerLevel.Error);

                //SocketInfo.RemoveClient(state);
                CloseSocket(handler);
            }
            catch (ObjectDisposedException)
            {
                //SocketInfo.RemoveClient(state);
                logger.AddtoLog("Socket has been closed", LoggerLevel.Error);
            }
        }

        /// <summary>
        /// Close Socket
        /// </summary>
        /// <param name="socket"></param>
        private void CloseSocket(Socket socket)
        {
            try
            {
                if (socket != null)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }
            catch { }
        }

        /// <summary>
        /// Close all connection
        /// </summary>
        private void CloseAll()
        {
            foreach (StateObject stat in SocketInfo.ListClient())
            {
                CloseSocket(stat.ClientSocket);
                SocketInfo.RemoveClient(stat);
            }

        }

        /// <summary>
        /// Close socket connection
        /// </summary>
        public void Close()
        {
            IsClosed = true;
            AllDone.Set();
            ///Wait till Service socke is stopped
            if (socketTask !=null ) socketTask.Wait();

            try
            {
                if (ListenerServer != null)
                {
                    if (ListenerServer.Connected)
                    {
                        ListenerServer.Shutdown(SocketShutdown.Both);
                    }
                    ListenerServer.Close();
                }
            }
            catch (SocketException) { ListenerServer = null; }
            catch (ObjectDisposedException) { ListenerServer = null; }
            CloseAll();
        }
    }
}
