using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asiacell.ITADLibraries_v1.LibDatabase;
using Oracle.DataAccess.Client;
using Asiacell.ITADLibraries_v1.Utilities;


namespace Asiacell.ITADLibraries_v1.LibSocket
{
    sealed class ManageClientCommand : IDisposable
    {
        private DataStoreCommand CommandStore = null;
        private ClientInfo socketClient = null;
        private DBConnectionPool db = null;
        private Task task = null;
        private volatile bool IsDisposed = false;

        private OracleCommand oraCommand = null;
        private OracleDataReader oraReader = null;

        public ManageClientCommand(ClientInfo clients, DataStoreCommand store, DBConnectionPool db)
        {
            this.CommandStore = store;
            this.socketClient = clients;
            this.db = db;


        }


        public void StartManagTask()
        {
            task = Task.Factory.StartNew(()=>PeformCommand() , TaskCreationOptions.LongRunning);
        }


        private void PeformCommand()
        {
            oraCommand = db.GetConnection().GetCommand();
            
            while (!IsDisposed)
            {
                //CommandProperties command = CommandStore.GetReceiverData();

                
            }
        }        


        public void Dispose()
        {
            IsDisposed = true;
            task.Wait();

        }
    }
}
