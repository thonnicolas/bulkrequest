using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace Asiacell.ITADLibraries.LibServerSocket
{
    class ClientSocket
    {
        private Socket client = null;
        private long _SocketID = 1;
        private string _UserName = string.Empty;
        private DateTime Connected_Date;
        private Stream stream;

        public ClientSocket(Socket socket, string UserName, int Send_Timeout, int Receive_Timeout)
        {
            this.client = socket;
            this._UserName = UserName;
            this.Connected_Date = DateTime.Now;
            _SocketID = GenerateSocketID.GetSocketID;

            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, Send_Timeout);
            client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Receive_Timeout);
            stream = new BufferedStream(new NetworkStream(socket, false));
        }

        public Socket Socket { get { return client; } }
        public string UserName { get { return _UserName; } }
        public long ID { get { return _SocketID; } }

        public Stream ClientStream { get { return stream; } }

        public void RestStream()
        {
            stream = new BufferedStream(new NetworkStream(client, false));
        }

    }
}
