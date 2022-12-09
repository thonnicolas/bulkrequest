using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Asiacell.ITADLibraries_v1.Utilities;
using System.Text;

namespace Asiacell.ITADLibraries_v1.Sockets
{
    public sealed class SocketClient : IDisposable
    {
        object locker = new object();
        private readonly Logger logger = null;
        private string IP;
        private int Port;

        private IPEndPoint endPoint;
        private Socket client;
        private NetworkStream networkStream;
        //private StreamReader streamReader = null;

        private int BufferSize = 2048;

        private volatile bool isUsed = false;
        private DateTime Created;
        private int sendReceiveTimeout = 2000;

        internal int SendReceiveTimeout { get { return sendReceiveTimeout; } set { sendReceiveTimeout = value; } }
        internal bool IsUsed { get { lock (locker) return isUsed; } set { lock (locker) isUsed = value; } }

        /// <summary>
        /// Get socket created date and time
        /// </summary>
        public DateTime GetCreatedDate { get { return Created; } }

        /// <summary>
        /// Initial the socket connection
        /// </summary>
        /// <param name="IP">Host IP</param>
        /// <param name="Port">Port</param>
        /// <param name="sendReceiveTimeout">Send/Receive time out in Milisecond</param>
        /// <param name="BufferSize">socket Stream buffer size default : 2048</param>
        public SocketClient(string IP, int Port, int sendReceiveTimeout, int BufferSize)
        {

            //this.socketID = SocketID;

            logger = new Logger(GetType());
            this.IP = IP;
            this.Port = Port;
            this.BufferSize = BufferSize;
            this.sendReceiveTimeout = sendReceiveTimeout;
            endPoint = getEndPoint(IP, Port);

            isUsed = false;

        }


        /// <summary>
        /// Create socket instant and open connection
        /// </summary>
        public bool Open()
        {
            try
            {
                // Create connection instant
                logger.Info("Create client socket connection : " + endPoint.Address.ToString());

                Created = DateTime.Now;
                //Set up the socket.
                //If this happened after 15 but before 100 seconds, the server responded with a connection closed error.  
                //To prevent server response error 
                ServicePointManager.MaxServicePointIdleTime = 10000;
                client = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, sendReceiveTimeout);
                client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, sendReceiveTimeout);
                client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, false);
                client.ReceiveTimeout = sendReceiveTimeout;
                client.SendTimeout = sendReceiveTimeout;
                //Do not use Nagle's Algorithm
                client.NoDelay = true;
                client.SendBufferSize = BufferSize;
                client.ReceiveBufferSize = BufferSize;
                //Establish connection

                client.Connect(endPoint);
                logger.Info("The connection is established... :" + endPoint.Address.ToString());
                //Wraps two layers of streams around the socket for communication.
                networkStream = new NetworkStream(client, false);

                //Used Flag
                isUsed = false;

                return true;
            }
            catch (SocketException ex)
            {
                logger.Error("Unable to open connection to " + IP + ", Error:" + ex.Message);
                //logger.Error(ex, ex);
            }
            catch (Exception ex)
            {
                logger.Error("Unable to open connection to " + IP + ", Error:" + ex.Message);
                //logger.Error(ex, ex);
            }

            client = null;
            networkStream = null;

            return false;
        }



        /// <summary>
        /// Get Network Stream
        /// </summary>
        public NetworkStream ConnectionStream
        {
            get
            {
                if (IsAlive)
                    return networkStream;
                else
                    return null;
            }
        }

        /// <summary>
        /// This method parses the given string into an IPEndPoint.
        /// If the string is malformed in some way, or if the host cannot be resolved, this method will throw an exception.
        /// </summary>
        internal IPEndPoint getEndPoint(string ip, int port)
        {
            //Parse host string.
            IPAddress address;
            if (IPAddress.TryParse(ip, out address))
            {
                //host string successfully resolved as an IP address.
                logger.Info("Resolve IP :" + ip + " successful");
            }
            else
            {
                //See if we can resolve it as a hostname
                try
                {
                    address = Dns.GetHostEntry(ip).AddressList[0];
                }
                catch (Exception e)
                {
                    logger.Error("Unable to resolve host: " + ip + ", Error:" + e.Message);
                    return null;
                }
            }

            return new IPEndPoint(address, port);
        }


        /// <summary>
        /// Is connection alive
        /// </summary>
        public bool IsAlive
        {
            get { return (client != null && client.Connected && networkStream.CanRead); }
        }

        /// <summary>
        /// Writes a string to the socket encoded in ASCII format.
        /// </summary>
        public void Write(string str)
        {
            Write(Encoding.ASCII.GetBytes(str));
        }

        /// <summary>
        /// Writes an array of bytes to the socket and flushes the stream.
        /// </summary>
        public void Write(byte[] bytes)
        {
            try
            {
                networkStream.Write(bytes, 0, bytes.Length);
                networkStream.Flush();
            }
            catch (SocketException ex)
            {
                throw new Exception("Socket write data error : " + IP + ", Erro Code:" + ex.SocketErrorCode + ", Description:" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Socket write data error : " + IP + ", Erro Code:" + ex.Source + ", Description:" + ex.Message);
            }
        }

        /// <summary>
        /// Write an array of bytew with len to the socket and flushes the stream.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="len"></param>
        public void Write(byte[] bytes, int len)
        {
            try
            {
                networkStream.Write(bytes, 0, len);
                networkStream.Flush();
            }
            catch (SocketException ex)
            {
                throw new Exception("Socket write data error : " + IP + ", Erro Code:" + ex.SocketErrorCode + ", Description:" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Socket write data error : " + IP + ", Erro Code:" + ex.Source + ", Description:" + ex.Message);
            }
        }

        /// <summary>
        /// Reads from the socket until the sequence '\n' is encountered, 
        /// and returns everything up to but not including that sequence as a ASCII-encoded string
        /// </summary>
        public string ReadLine()
        {
            try
            {
                StreamReader streamReader = new StreamReader(networkStream);
                return streamReader.ReadLine();
            }
            catch (SocketException ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.SocketErrorCode + ", Description:" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.Source + ", Description:" + ex.Message);
            }
            //MemoryStream buffer = new MemoryStream();           

            //int b;
            //while ((b = networkStream.ReadByte()) != -1)
            //{
            //    if (b == 13)
            //    {
            //        buffer.WriteByte(13);
            //        break;
            //    }
            //    else
            //    {
            //        buffer.WriteByte((byte)b);
            //    }
            //}
            //return Encoding.ASCII.GetString(buffer.GetBuffer(), 0, Convert.ToInt32(buffer.Length.ToString()));
        }

        /// <summary>
        /// Read data from buffer till "EndString" with last "To be continued..."
        /// </summary>
        /// <param name="EndString"></param>
        /// <returns></returns>
        public string ReadToEndString(string EndString, string LastEnd)
        {

            MemoryStream buffer = new MemoryStream();
            string result = string.Empty;
            try
            {
                int b;
                int startcontinue = 0;
                int endcountinue = 0;

                Byte[] bytesReceived = new Byte[BufferSize];

                if (!IsAlive) return "Connection failed";
                do
                {

                    //b = networkStream.Read(bytesReceived, 0, bytesReceived.Length);
                    //networkStream.Flush();
                    networkStream = new NetworkStream(client, true);

                    b = networkStream.Read(bytesReceived, 0, bytesReceived.Length);

                    result += Encoding.ASCII.GetString(bytesReceived, 0, b);
                    if (result.IndexOf(LastEnd) != -1)
                    {
                        startcontinue++;
                        result = result.Replace(LastEnd, "");
                    }
                    else if (result.IndexOf(EndString) != -1)
                    {
                        if (endcountinue == startcontinue) break;
                        result = result.Replace(EndString, "");
                        endcountinue++;
                    }
                } while (b > 0);
            }
            catch (SocketException ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.SocketErrorCode + ", Description:" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.Source + ", Description:" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Read data from buffer till "EndString"
        /// </summary>
        /// <param name="EndString"></param>
        /// <returns></returns>
        public string ReadToEndString(string EndString)
        {

            MemoryStream buffer = new MemoryStream();
            string result = string.Empty;
            try
            {
                int b;

                Byte[] bytesReceived = new Byte[BufferSize];

                if (!IsAlive) return "Connection failed";
                do
                {

                    b = client.Receive(bytesReceived);

                    //b = networkStream.Read(bytesReceived, 0, bytesReceived.Length);
                    //networkStream.Flush();
                    result += Encoding.ASCII.GetString(bytesReceived, 0, b);
                    if (result.IndexOf(EndString) != -1)
                        break;
                } while (b > 0);
            }
            catch (SocketException ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.SocketErrorCode + ", Description:" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.Source + ", Description:" + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Read to end;
        /// </summary>
        /// <returns></returns>
        public string ReadToEnd()
        {

            string result = string.Empty;

            try
            {
                StreamReader streamReader = new StreamReader(networkStream);
                result = streamReader.ReadToEnd();
            }
            catch (SocketException ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.SocketErrorCode + ", Description:" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.Source + ", Description:" + ex.Message);
            }
            return result;
        }


        /// <summary>
        /// Read data from buffer till "EndString"
        /// </summary>
        /// <param name="EndString"></param>
        /// <returns></returns>
        //public string ReadToEndString(string EndString)
        //{
        //    string result = string.Empty;
        //    string data;

        //    while ((data = ReadLine()) != null)
        //    {
        //        if (data.IndexOf(EndString) != -1)
        //        {
        //            result = result + data ;
        //            break;
        //        }
        //        else
        //        {
        //            result = result + data;
        //        }
        //    }

        //    return result;
        //}

        /// <summary>
        /// Fills the given byte array with data from the socket.
        /// </summary>
        public void Read(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            try
            {
                int readBytes = 0;
                while (readBytes < bytes.Length)
                {
                    readBytes += networkStream.Read(bytes, readBytes, (bytes.Length - readBytes));
                }

            }
            catch (SocketException ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.SocketErrorCode + ", Description:" + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Socket read data error : " + IP + ", Erro Code:" + ex.Source + ", Description:" + ex.Message);
            }

        }

        /// <summary>
        /// Reads from the socket until the sequence '\r\n' is encountered.
        /// </summary>
        public void SkipUntilEndOfLine()
        {
            int b;
            bool gotReturn = false;
            while ((b = networkStream.ReadByte()) != -1)
            {
                if (gotReturn)
                {
                    if (b == 10)
                    {
                        break;
                    }
                    else
                    {
                        gotReturn = false;
                    }
                }
                if (b == 13)
                {
                    gotReturn = true;
                }
            }
        }

        /// <summary>
        /// Resets this Socket by making sure the incoming buffer of the socket is empty.
        /// If there was any leftover data, this method return true.
        /// </summary>
        public bool Reset()
        {
            if (client.Available > 0)
            {
                byte[] b = new byte[client.Available];
                Read(b);
                isUsed = false;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Dispose connection
        /// </summary>
        public void Dispose()
        {
            if (networkStream != null)
            {
                try { networkStream.Close(); }
                catch (Exception e)
                {
                    logger.Error("Error closing stream: " + IP);
                }
                networkStream = null;
            }
            if (client != null)
            {
                try { client.Shutdown(SocketShutdown.Both); }
                catch (Exception e)
                {
                    logger.Error("Error shutting down socket: " + IP);
                }
                try { client.Close(); }
                catch (Exception e)
                {
                    logger.Error("Error closing socket: " + IP);
                }
                client = null;
            }

            isUsed = false;
        }
    }
}
