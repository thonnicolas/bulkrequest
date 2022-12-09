using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;

namespace Asiacell.ITADLibraries.Utilities
{
    public class ElementProperties
    {
        public string Name { get; set; }
        public int ElementID { get; set; }
        public int Elementtypeid { get; set; }
        public string IP { get; set; }
        public int Port { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string LogIn_Command { get; set; }
        public string LogOut_Command { get; set; }
        public string Encryption { get; set; }
        public int VersionID { get; set; }
        public int SendReceive_TimeOut { get; set; }
        public int BufferSize { get; set; }
        public int HandShakeInterval { get; set; }
        public int MAX_PoolSize { get; set; }
        public int MIN_PoolSize { get; set; }
        public object param1 { get; set; }
        public object param2 { get; set; }
        public object param3 { get; set; }
        public object param4 { get; set; }
        public object param5 { get; set; }
        public object param6 { get; set; }
        public object param7 { get; set; }
        public object param8 { get; set; }
        public object param9 { get; set; }

        public ElementProperties()
        {

        }
        ///// <summary>
        ///// Properties element contractor
        ///// </summary>
        ///// <param name="ElmentID">Element ID</param>
        ///// <param name="IP">Element Host IP</param>
        ///// <param name="Port">Element Port</param>
        ///// <param name="UserID">Element user login </param>
        ///// <param name="Password">Element password login</param>
        ///// <param name="LogIn_Command">Login command string</param>
        ///// <param name="LogOut_Command">Logout command string</param>
        ///// <param name="Encryption">Encryption prefix</param>
        ///// <param name="VersionID">Version ID</param>
        ///// <param name="SendReceive_TimeOut">Send/Received Time out interval in milisecond</param>
        ///// <param name="BufferSize">Socket buffer size by default 1024</param>
        ///// <param name="HandShakeInterval">Hand shake time interval</param>
        //public ElementProperties(int ElmentID, string IP, int Port, string UserID, string Password,
        //    string LogIn_Command, string LogOut_Command, string Encryption, int VersionID,
        //    int SendReceive_TimeOut, int BufferSize, int HandShakeInterval, int MaxPoolSize, int MinPoolSize, bool KeepAlive)
        //{
        //    this.ElmentID = ElmentID;
        //    this.IP = IP;
        //    this.Port = Port;
        //    this.UserID = UserID;
        //    this.Password = Password;
        //    this.LogIn_Command = LogIn_Command.Replace("%USR%", UserID).Replace("%PWD%", Password);
        //    this.LogOut_Command = LogOut_Command;
        //    this.Encryption = Encryption;
        //    this.VersionID = VersionID;
        //    this.SendReceive_TimeOut = SendReceive_TimeOut;
        //    this.BufferSize = BufferSize;
        //    this.HandShakeInterval = HandShakeInterval;
        //    this.MAX_PoolSize = MaxPoolSize;
        //    this.MIN_PoolSize = MinPoolSize;
        //    this.KeepAlive = KeepAlive;
        //}



        /// <summary>
        /// 
        /// </summary>
        /// <param name="ElmentID"></param>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        /// <param name="UserID"></param>
        /// <param name="Password"></param>
        /// <param name="LogIn_Command"></param>
        /// <param name="LogOut_Command"></param>
        /// <param name="Encryption"></param>
        /// <param name="VersionID"></param>
        /// <param name="SendReceive_TimeOut"></param>
        /// <param name="BufferSize"></param>
        /// <param name="HandShakeInterval"></param>
        /// <param name="MAX_PoolSize"></param>
        /// <param name="MIN_PoolSize"></param>
        public ElementProperties(int ElmentID, string IP, int Port, string UserID, string Password, string LogIn_Command,
            string LogOut_Command, string Encryption, int VersionID, int SendReceive_TimeOut,
            int BufferSize, int HandShakeInterval, int MAX_PoolSize, int MIN_PoolSize)
        {
            this.ElementID = ElmentID;
            this.IP = IP;
            this.Port = Port;
            this.UserID = UserID;
            this.Password = Password;
            this.LogIn_Command = Functions.ToString(LogIn_Command).Replace("%USR%", UserID).Replace("%PWD%", Password);
            this.LogOut_Command = LogOut_Command;
            this.Encryption = Encryption;
            this.VersionID = VersionID;
            this.SendReceive_TimeOut = SendReceive_TimeOut;
            this.BufferSize = BufferSize;
            this.HandShakeInterval = HandShakeInterval;
            this.MAX_PoolSize = MAX_PoolSize;
            this.MIN_PoolSize = MIN_PoolSize;
        }

        public ElementProperties(int ElmentID, string IP, int Port, string UserID, string Password, string LogIn_Command,
            string LogOut_Command, string Encryption, int VersionID, int SendReceive_TimeOut,
            int BufferSize, int HandShakeInterval, int MAX_PoolSize, int MIN_PoolSize, object p1, object p2, object p3, object p4, object p5, object p6, object p7, object p8, object p9)
        {
            this.ElementID = ElmentID;
            this.IP = IP;
            this.Port = Port;
            this.UserID = UserID;
            this.Password = Password;
            this.LogIn_Command = Functions.ToString(LogIn_Command).Replace("%USR%", UserID).Replace("%PWD%", Password);
            this.LogOut_Command = LogOut_Command;
            this.Encryption = Encryption;
            this.VersionID = VersionID;
            this.SendReceive_TimeOut = SendReceive_TimeOut;
            this.BufferSize = BufferSize;
            this.HandShakeInterval = HandShakeInterval;
            this.MAX_PoolSize = MAX_PoolSize;
            this.MIN_PoolSize = MIN_PoolSize;
            this.param1 = p1;
            this.param2 = p2;
            this.param3 = p3;
            this.param4 = p4;
            this.param5 = p5;
            this.param6 = p6;
            this.param7 = p7;
            this.param8 = p8;
            this.param9 = p9;
        }
    }
}
