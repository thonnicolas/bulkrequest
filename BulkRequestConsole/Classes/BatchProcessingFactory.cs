using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.LibDatabase;
using Asiacell.ITADLibraries.LibLogger;
using BulkRequestConsole.Classes;

namespace BulkRequestConsole.Classes
{
 
    public class BatchProcessingFactory : IBulkRequesterFactory
    {
        DBConnection db;
        LoggerEntities logger;

        public BatchProcessingFactory(DBConnection db, LoggerEntities logger)
        {
            this.db = db;
            this.logger = logger;
        }

        public IBulkReqester Create()
        {
            return new BatchProcessingLogic(logger, db);
        }
    }
}
