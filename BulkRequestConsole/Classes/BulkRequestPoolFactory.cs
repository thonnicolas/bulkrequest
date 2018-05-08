using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibWCF.Service;
using Asiacell.ITADLibraries.LibDatabase;
using System.Collections.Concurrent;

namespace BulkRequestConsole.Classes
{
    public class BulkRequestPoolFactory: IFactoryCreator
    {
        LoggerEntities logger;
        DBConnection db;
        string serviceUrl;

        string GWUser;
        string GWPassword;
        string migrateTable;

        IBulkRequesterFactory requesterFactory;

        public BulkRequestPoolFactory(LoggerEntities logger, string serviceUrl,    DBConnection db, string gWUser, string gWPassword, string migrateTable, IBulkRequesterFactory requesterFactory)
        {
            this.logger = logger;             
            this.serviceUrl = serviceUrl;
      
            
            this.db = db;
            this.GWUser = gWUser;
            this.GWPassword = gWPassword;
            this.migrateTable = migrateTable;
            this.requesterFactory = requesterFactory;
        }

        public IConnectionControllerPool Create(BlockingCollection<object> idleworker)
        {
            return new BulkRequestPool(idleworker, this.logger, this.serviceUrl, this.db, this.GWUser, this.GWPassword, this.migrateTable, this.requesterFactory);
        }
    }
}
