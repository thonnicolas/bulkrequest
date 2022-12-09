
namespace Asiacell.ITADLibraries_v1.LibSocketClient
{
    public class ClientProperties
    {
        public readonly string UserName;
        public readonly string Password;
        public readonly string Server_IP;
        public readonly int Server_Port;
        public readonly int InquiryLinkInterval = 0;
        public readonly bool KeepAlive = false;
        public readonly int BufferSize = 1024;
        public readonly int ReadWriteTimeOut = 60000;


        public ClientProperties(string Server_IP, int Server_Port ,string UserName, string Password)
        {
            this.UserName = UserName;
            this.Password = Password;
            this.Server_IP = Server_IP;
            this.Server_Port = Server_Port;
        }

        public ClientProperties(string Server_IP, int Server_Port, string UserName, string Password, int InquiryLinkInterval)
        {
            this.UserName = UserName;
            this.Password = Password;
            this.Server_IP = Server_IP;
            this.Server_Port = Server_Port;
            this.InquiryLinkInterval = InquiryLinkInterval;
        }

        public ClientProperties(string Server_IP, int Server_Port, string UserName, string Password,bool KeepAlive, int BufferSize, int ReadWriteTimeout, int InquiryLinkInterval)
        {
            this.UserName = UserName;
            this.Password = Password;
            this.Server_IP = Server_IP;
            this.Server_Port = Server_Port;
            this.KeepAlive = KeepAlive;
            this.BufferSize = BufferSize;
            this.ReadWriteTimeOut = ReadWriteTimeout;
            this.InquiryLinkInterval = InquiryLinkInterval;
        }

    }
}
