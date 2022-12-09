using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace Asiacell.ITADLibraries.LibSocketClient
{
    class ClientSocket
    {
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        //private ManualResetEvent WaitRespond = new ManualResetEvent(false);

        private IPEndPoint destination = null;

        public BlockingCollection<string> messagelog = null;
        private string UserName = string.Empty;
        private string PWD = string.Empty;
        private int TransactionID = 0;


        private Socket client;
        
        public ClientSocket(string ServerIP, int ServerPort, string UserID, string Password)
        {            
            destination = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);

            this.UserName = UserID;
            this.PWD = Password;
            messagelog = new BlockingCollection<string>();
        }

        //private int GetCommandID()
        //{
        //    return Interlocked.Increment(ref TransactionID);
        //}


        /// <summary>
        /// 
        /// </summary>
        public void StartConnect()
        {
            
            try
            {
                client = new Socket(destination.AddressFamily, SocketType.Stream, ProtocolType.Tcp);                
                client.BeginConnect(destination, new AsyncCallback(OnConnecting), client); 
                connectDone.WaitOne();
            }
            catch (SocketException ex)
            {
                AddToLog("Socket has been closed : ErrorCode : " + ex.ErrorCode + ", Description : " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                AddToLog("Socket has been closed");
            }

        }       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void OnConnecting(IAsyncResult ar)
        {
            Socket socket = null;
            
            try
            {
                socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);

                AddToLog("Client connected to " + ((IPEndPoint)socket.RemoteEndPoint).Address.ToString());

                StateObject state = new StateObject();
                state.ClientSocket = socket;
                socket.BeginReceive(state.buffer,0,state.BufferSize, SocketFlags.None, new AsyncCallback(Login), state);
                
            }
            catch (SocketException ex)
            {
                AddToLog("Socket has been closed : ErrorCode : " + ex.ErrorCode + ", Description : " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                AddToLog("Socket has been closed");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        private void Login(IAsyncResult ar)
        {
            StateObject state = null;
            Socket socket = null;
            string CommandID = string.Empty;
            try
            {
                state = (StateObject)ar.AsyncState;
                socket = state.ClientSocket;
                int byteRead = socket.EndReceive(ar);
                String content = String.Empty;

                if (byteRead > 0)
                { 
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer,0, byteRead));
                    content = state.sb.ToString();
                    if (content.IndexOf(ServerCommandConstant.EndCommand) > -1)
                    {
                        state.sb.Clear();
                        CommandRespondProperties respond = SocketCommand.IsResponcommand(content);

                        if (respond != null)
                        {
                            if (respond.ResultCode == ServerCommandConstant.Successful)
                            {
                                AddToLog(respond.Result);
                                socket.BeginReceive(state.buffer, 0, state.BufferSize, SocketFlags.None, new AsyncCallback(Recevier), state);

                            }                            
                            else // Faile
                            {
                                AddToLog(respond.Result);
                            }
                            connectDone.Set();
                            return;
                        }
                        else
                        {
                            CommandID = SocketCommand.GenCommandID;
                            AddToLog(content);
                            Send(SocketCommand.GenLoginCommand(UserName, PWD, CommandID));
                        }
                    }
                    socket.BeginReceive(state.buffer, 0, state.BufferSize, SocketFlags.None, new AsyncCallback(Login), state);
                }

            }
            catch (SocketException ex)
            {
                AddToLog("Socket has been closed : ErrorCode : " + ex.ErrorCode + ", Description : " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                AddToLog("Socket has been closed");
            }
        }


        /// <summary>
        /// Receiver CallBack
        /// </summary>
        /// <param name="ar"></param>
        private void Recevier(IAsyncResult ar)
        {
            StateObject state = null;
            try
            {
                state = (StateObject)ar.AsyncState;
                Socket socket = state.ClientSocket;
                int byteRead = socket.EndReceive(ar);
                String content = String.Empty;

                if (byteRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, byteRead));
                    content = state.sb.ToString();
                }
                else
                { 
                    ///Connection lose
                    ///
                    AddToLog("The connection lose");
                    return;
                }

                socket.BeginReceive(state.buffer, 0, state.BufferSize,SocketFlags.None, new AsyncCallback(Recevier), state);
                
            }
            catch (SocketException ex)
            {
                AddToLog("Socket has been closed : ErrorCode : " + ex.ErrorCode + ", Description : " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                AddToLog("Socket has been closed");
            }

        }

        /// <summary>
        /// Send to client
        /// </summary>
        /// <param name="state"></param>
        /// <param name="data"></param>
        public void Send(String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            //data = data + ServerCommandConstant.EndCommand;
            byte[] byteData = SocketCommand.GenerateCommand(data, SocketCommand.GenCommandID);
            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), null);            
        }


        public void Send(byte[] data)
        {
            // Begin sending the data to the remote device.
            client.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(SendCallback), null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void AddToLog(String message)
        {
            Task.Factory.StartNew(delegate { messagelog.Add(message); }, message, TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// Send CallBack
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.
            try
            { 
                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
            }
            catch (SocketException ex)
            {
                AddToLog("Socket has been closed : ErrorCode : " + ex.ErrorCode + ", Description : " + ex.Message);
            }
            catch (ObjectDisposedException)
            {

            }
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            string CommandID = SocketCommand.GenCommandID;
            try
            {
                if (client != null)
                {
                    client.Send(SocketCommand.GenerateLogout(CommandID));
                    client.Shutdown(SocketShutdown.Both);
                    client.Close();
                }
            }
            catch (SocketException ex)
            {
                AddToLog("Socket has been closed : ErrorCode : " + ex.ErrorCode + ", Description : " + ex.Message);
            }
            catch (ObjectDisposedException)
            {
                AddToLog("Socket has been closed");
            }
        }

    }
}
