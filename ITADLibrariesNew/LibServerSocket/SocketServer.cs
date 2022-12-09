using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;          // For IPAddress
using System.Net.Sockets;  // For TcpListener, TcpClient
using Asiacell.ITADLibraries_v1.LibLogger;
using Asiacell.ITADLibraries_v1.LibDatabase;



namespace Asiacell.ITADLibraries_v1.LibServerSocket
{
    class SocketServer:IDisposable
    {
        private LoggerEntities logger = null;
        private DBConnectionPool db = null;
        private int ServerPort = 2013;
        private Socket listener = null;

        private bool IsDisposed = false;

        public SocketServer(int ServerPort,LoggerEntities logger, DBConnectionPool db)
        {
            this.logger = logger;
            this.db = db;
            this.ServerPort = ServerPort;
        }


        public void Start()
        {

            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the 
            // host running the application.
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);




            //// Bind the socket to the local endpoint and 
            //// listen for incoming connections.
            //try
            //{
            //    listener.Bind(localEndPoint);
            //    listener.Listen(10);



            //    // Start listening for connections.
            //    while (true)
            //    {
            //        Console.WriteLine("Waiting for a connection...");
            //        // Program is suspended while waiting for an incoming connection.
            //        Socket handler = listener.Accept();
            //        data = null;

            //        // An incoming connection needs to be processed.
            //        while (true)
            //        {
            //            bytes = new byte[1024];
            //            int bytesRec = handler.Receive(bytes);
            //            data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
            //            if (data.IndexOf("<EOF>") > -1)
            //            {
            //                break;
            //            }
            //        }

            //        // Show the data on the console.
            //        Console.WriteLine("Text received : {0}", data);

            //        // Echo the data back to the client.
            //        byte[] msg = Encoding.ASCII.GetBytes(data);

            //        handler.Send(msg);
            //        handler.Shutdown(SocketShutdown.Both);
            //        handler.Close();
            //    }

            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.ToString());
            //}
            
        }

        //public string GetIPAddress()
        //{
        //    //try
        //    //{
        //    //    // Establish the local endpoint for the socket.
        //    //    // Dns.GetHostName returns the name of the 
        //    //    // host running the application.
               
        //    //}
        //    //catch
        //    //{ }

        //    //return SysProperties.Server_IP;

        //}

        public void Dispose()
        {
        }
            
    }
}
