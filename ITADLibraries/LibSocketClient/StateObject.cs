using System.Text;
using System.Net.Sockets;

namespace Asiacell.ITADLibraries.LibSocketClient
{
    public class StateObject
    {
        // Client socket.
        public Socket ClientSocket = null;
        // Size of receive buffer.
        public int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[1024];
        // Received data string.
        public StringBuilder sb = new StringBuilder();



    }
}
