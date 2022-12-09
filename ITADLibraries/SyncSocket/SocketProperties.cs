using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asiacell.ITADLibraries.Sockets
{
    public class SocketProperties
    {
        /// <summary>
        /// Host IP
        /// </summary>
        public readonly string IP;
        /// <summary>
        /// Host Port
        /// </summary>
        public readonly int Port;
        /// <summary>
        /// Keep connection alive
        /// </summary>
        public readonly bool KeepAlive;
        /// <summary>
        /// Read/Write Time out
        /// </summary>
        public readonly int ReadWrite_TimeOut;
        /// <summary>
        /// User Login ID
        /// </summary>
        public readonly string UserID;
        /// <summary>
        /// Password Login ID
        /// </summary>
        public readonly string Password;
        /// <summary>
        /// Handshake interval
        /// </summary>
        public readonly int HandShake_Interval;

        public readonly int BufferSize;

        public SocketProperties(string IP, int Port, string UserID, string Password, bool KeepAlive, int BufferSize, int ReadWrite_TimeOut, int HandShake_Interval)
        {
            this.IP = IP;
            this.Port = Port;
            this.UserID = UserID;
            this.Password = Password;
            this.KeepAlive = KeepAlive;
            this.ReadWrite_TimeOut = ReadWrite_TimeOut;
            this.HandShake_Interval = HandShake_Interval;
            this.BufferSize = BufferSize;
        }

    }
}
