using System;
using System.Collections.Concurrent;
using Asiacell.ITADLibraries_v1.LibLogger;


namespace Asiacell.ITADLibraries_v1.LibSocketClient
{
    public abstract class ITClients: IDisposable
    {
        
        private SocketClient client;
        public ConcurrentQueue<String> tranLog = null;
        private LoggerEntities logger = null;

        private ClientProperties properties = null;

        /// <summary>
        /// Contractor without set send/receive timeout 
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        /// <param name="UserID"></param>
        /// <param name="Password"></param>
        /// <param name="Keepalive"></param>
        /// <param name="BufferSize"></param>
        /// <param name="InquiryLinkInterval"></param>
        public ITClients(string IP, int Port, string UserID, string Password, bool Keepalive, int BufferSize, int InquiryLinkInterval)
        {
            tranLog = new ConcurrentQueue<string>();
            logger = new LoggerEntities(tranLog);
            SetPara(IP, Port, UserID, Password, Keepalive, BufferSize, 60000, InquiryLinkInterval);

        }

        /// <summary>
        /// Contractor with set send/receive timeout 
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        /// <param name="UserID"></param>
        /// <param name="Password"></param>
        /// <param name="Keepalive"></param>
        /// <param name="BufferSize"></param>
        /// <param name="InquiryLinkInterval"></param>
        public ITClients(string IP, int Port, string UserID, string Password, bool Keepalive, int BufferSize,int ReadWriteTimeOut, int InquiryLinkInterval)
        {
            tranLog = new ConcurrentQueue<string>();
            logger = new LoggerEntities(tranLog);
            SetPara(IP, Port, UserID, Password, Keepalive, BufferSize, ReadWriteTimeOut, InquiryLinkInterval);

        }

        /// <summary>
        /// The Contractor, this contractor withour keep Alive link
        /// </summary>
        /// <param name="IP">ITMDW IP</param>
        /// <param name="Port">ITMDW Port</param>
        /// <param name="UserID">User login</param>
        /// <param name="Password">Password</param>
        public ITClients(string IP, int Port, string UserID, string Password)
        {
            logger = new LoggerEntities(tranLog);
            SetPara(IP, Port, UserID, Password, false, 1024, 60000, 0);
        }

        /// <summary>
        /// Set value to required paramaters
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="Port"></param>
        /// <param name="UserID"></param>
        /// <param name="Password"></param>
        /// <param name="KeepAlive"></param>
        /// <param name="BufferSize"></param>
        /// <param name="ReadWrite_TimeOut"></param>
        /// <param name="HandShake_Interval"></param>
        private void SetPara(string IP, int Port, string UserID, string Password, bool KeepAlive, int BufferSize, int ReadWriteTimeOut, int HandShakeInterval)
        {
            properties = new ClientProperties(IP, Port, UserID, Password, KeepAlive, BufferSize, ReadWriteTimeOut, HandShakeInterval);

            client = new SocketClient(properties, logger);

        }

        /// <summary>
        /// Login to Middleware
        /// </summary>
        /// <returns></returns>
        public bool LogIn(out string error)
        {
            bool IsSucceed = false;
            string result = string.Empty;
            try
            {
                logger.AddtoLog("","Loging to ITMDW bye ueser : " + properties.UserName, LoggerLevel.Info);

                if (client != null && client.Login())
                {
                    error = "Loging to ITMDW sucessfuly";
                    IsSucceed = true;
                }
                else
                {
                    error = "Unable to connect to ITMDW : " + result;
                    logger.AddtoLog("",error,LoggerLevel.Info);
                }
            }
            catch (Exception ex)
            {
                error = "Unable to connect to ITMDW :"+ ex.Message;
                logger.AddtoLog("", error , LoggerLevel.Error);
            }

            return IsSucceed;
        }




        public void Dispose()
        {
            //IsRequestStop = true;

            //logger.Info("Reqeust to stop process.... please wait ");
            //{
            //    //Set Null to exit internal Thread
            //    task.Add(null);
            //    InternalTask.Join(TimeSpan.FromSeconds(20));
            //}

            //logger.Info("The task is stoped successful");
        }
    }
}
