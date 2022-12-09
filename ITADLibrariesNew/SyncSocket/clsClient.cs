using System;              // For String, Int32, Console, ArgumentException
using System.Text;         // For Encoding
using System.IO;           // For IOException
using System.Net.Sockets;  // For Socket, SocketException
using System.Net;          // For IPAddress, IPEndPoint


namespace Asiacell.ITADLibraries_v1.Sockets
{
    class clsClient
    {
        public string Err = "";
        public int ErrID = 0;

        Socket sock = null;
        private const int BUFSIZE = 1024; // Size of receive buffer

        private const string terminator = "-;end string;-";
        private const string exit_command = "--+bye bye+--";
        private const string exit_command_confirmation = "--+bye ok+--";

        private const int send_timeout = 3000;
        private const int receive_timeout = 3000;


        public clsClient(string _IP, int _Port)
        {
            Init(_IP, _Port, send_timeout, receive_timeout);
        }

        public clsClient(string _IP, int _Port, int Send_Timeout, int Receive_Timeout)
        {
            Init(_IP, _Port, Send_Timeout, Receive_Timeout);

        }

        private void Init(string _IP, int _Port, int Send_Timeout, int Receive_Timeout)
        {
            IPAddress localAddr = IPAddress.Parse(_IP);

            IPEndPoint serverEndPoint = new IPEndPoint(localAddr, _Port);

            try
            {
                // Create a TCP socket instance 
                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, Send_Timeout);
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Receive_Timeout);

                // Connect the socket to server on specified port
                sock.Connect(serverEndPoint);
            }
            catch (SocketException se)
            {
                Err = se.Message;
                ErrID = se.ErrorCode;
            }
            catch (Exception e)
            {
                Err = e.Message;
            }
        }

        public bool SendText(string text)
        {
            try
            {
                if (sock.Connected)
                {
                    text += terminator;
                    byte[] byteBuffer = Encoding.ASCII.GetBytes(text);

                    // Send the encoded string to the server
                    sock.Send(byteBuffer, 0, byteBuffer.Length, SocketFlags.None);
                    return true;
                }
                else
                {
                    Err = "Socker was not connected";
                }

            }
            catch (SocketException se)
            {
                Err = se.Message;
                ErrID = se.ErrorCode;
                Console.WriteLine("SendText: " + se.ErrorCode + ": " + se.Message);
            }
            catch (SystemException ex)
            {
                Err = ex.Message;
            }

            return false;

        }

        public string ReceiveText()
        {
            int bytesRcvd;                        // Received byte count
            byte[] rcvBuffer = new byte[BUFSIZE]; // Receive buffer
            string res = "";

            try
            {
                while (sock.Connected)
                {
                    //bytesRcvd = client.Receive(rcvBuffer, 0, rcvBuffer.Length, SocketFlags.None);
                    bytesRcvd = sock.Receive(rcvBuffer);//bn, 0, rcvBuffer.Length, SocketFlags.None);

                    if (bytesRcvd > 0)
                    {
                        res += Encoding.ASCII.GetString(rcvBuffer, 0, bytesRcvd);

                        if (res.EndsWith(terminator))
                        {
                            res = res.Substring(0, res.Length - terminator.Length);
                        }
                        else
                            continue;
                    }
                    else
                    {
                    }

                    if (bytesRcvd < BUFSIZE)
                        break;
                }

                return res;
            }
            catch (SocketException se)
            {
                Console.WriteLine("ReceiveText: " + se.ErrorCode + ": " + se.Message);
                Err = se.Message;
                ErrID = se.ErrorCode;
            }
            catch (SystemException ex)
            {
                Err = ex.Message;
            }

            return "";

        }

        public bool Close()
        {

            try
            {
                SendText(exit_command);

                string res = ReceiveText();

                if (res.StartsWith(exit_command_confirmation))
                {
                    return true;
                }
                else
                {
                    Err = "bye bye confirmation was not received";
                }
            }
            catch
            {
            }
            finally
            {
                if (sock.Connected)
                {
                    try
                    {
                        sock.Shutdown(SocketShutdown.Both);
                        sock.Disconnect(false);
                    }
                    catch { }
                }

                sock.Close();
                sock.Dispose();
                GC.WaitForFullGCComplete();
            }

            return false;

        }

       


    }
}
