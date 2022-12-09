using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using Asiacell.ITADLibraries_v1.LibDatabase;
using Asiacell.ITADLibraries_v1.LibLogger;
using System.Threading.Tasks;

namespace Asiacell.ITADLibraries_v1.LibServerSocket
{
    class ClientSocketHandler
    {
        private ClientSocket Client = null;
        private DBConnection db = null;
        private LoggerEntities logger = null;



        public ClientSocketHandler(ClientSocket client, LoggerEntities logger , string DBConnectionString)
        {
            this.Client = client;
            this.logger = logger;

            db = new DBConnection(DBConnectionString, logger);
            db.StartupConnection();                 
        }


        /// <summary>
        /// Check whether current connection is still alive
        /// </summary>
        /// <returns></returns>
        public bool IsAlive()
        {
            try
            {
                if (Client != null)
                    return Client.Socket.Connected && Client.ClientStream.CanRead;
            }
            catch (SystemException e)
            {
                //Err = e.Message;
                //logger.Error("Service: " + ServiceName, e);
                //Console.WriteLine(e.Message);

                //ServerManager.RecordLog(ServiceName, UserName,
                //                        (ClientIP == "" ? ServerManager.GetIP(client) : ClientIP), "", -1,
                //                        "", "", Err, "", 0,
                //                        ITADLibraries.Utilities.SystemErrorCodes.Unknown_Error);
            }

            return false;
        }
        
    }
}
