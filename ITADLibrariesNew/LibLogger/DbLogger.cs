using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries_v1.Utilities;
using System.Collections.Concurrent;
using Oracle.DataAccess.Client;
using Oracle.DataAccess;
using Asiacell.ITADLibraries_v1.LibDatabase;
using System.Data;
using System.IO;
using System.ComponentModel;


namespace Asiacell.ITADLibraries_v1.LibLogger
{
    public class DbLogger<T> : ITaskExecutor, IDisposable where T : class
    {
        private int bulkBindCount;
        private DBDataTransaction bulkInsert;
        private TaskExecutorManager taskManager;
        private string logFilePath;
        private DateTime lastCall;
        private int recollectPeriod = 10000; // millisecond
        private bool isCancel = false;
        private LoggerEntities logger = null;
        private IAttributeValidator attributeValidator = null;
        private TransactionLogger<T> fileLogger = null;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLogger(DBConnection db, LoggerEntities logger, int bulkBindCount, string logFilePath, int recollectPeriod, IAttributeValidator attributeValidator, TransactionLogger<T> fileLogger)
        {

            Initialize(db, logger, bulkBindCount, logFilePath, recollectPeriod, "tbl_logger", false, null, attributeValidator, fileLogger);
   
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLogger(DBConnection db, LoggerEntities logger, int bulkBindCount, string logFilePath, int recollectPeriod, string table_name, bool useBulkCopy, IAttributeValidator attributeValidator, TransactionLogger<T> fileLogger)
        {
            Initialize(db, logger, bulkBindCount, logFilePath, recollectPeriod, table_name, useBulkCopy, null, attributeValidator,fileLogger);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLogger(DBConnection db, LoggerEntities logger, int bulkBindCount, string logFilePath, int recollectPeriod, TaskExecutorManager taskManager, IAttributeValidator attributeValidator, TransactionLogger<T> fileLogger)
        {
            Initialize(db, logger, bulkBindCount, logFilePath, recollectPeriod, "tbl_logger", false, taskManager,attributeValidator,fileLogger);            
        }

        /// <summary>
        /// Intialize values
        /// </summary>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        /// <param name="bulkBindCount"></param>
        /// <param name="logFilePath"></param>
        /// <param name="recollectPeriod"></param>
        /// <param name="table_name"></param>
        /// <param name="useBulkCopy"></param>
        /// <param name="taskExectorManager"></param>
        /// <param name="attributeValidator"></param>
        private void Initialize(DBConnection db, LoggerEntities logger, int bulkBindCount, string logFilePath, int recollectPeriod, string table_name, bool useBulkCopy, TaskExecutorManager taskExectorManager, IAttributeValidator attributeValidator, TransactionLogger<T> fileLogger)
        {
            this.bulkBindCount = bulkBindCount;
            this.bulkInsert = new DBDataTransaction(table_name, db, bulkBindCount, useBulkCopy, logger);
            this.recollectPeriod = recollectPeriod * 1000;
            if (taskExectorManager == null)
                this.taskManager = new TaskExecutorManager(this);
            else
            {
                this.taskManager = taskExectorManager;
                this.taskManager.SetExecutor(this);
            }
            this.logger = logger;
            this.logFilePath = logFilePath;
            this.lastCall = DateTime.Now;
            this.attributeValidator = attributeValidator;
            this.fileLogger = fileLogger;            
        }

        /// <summary>
        /// Add to task queues manager
        /// </summary>
        /// <param name="log"></param>
        public bool AddtoLogs(T log)
        {

            if (isCancel == true)
                return false;
            try
            {
                this.taskManager.AddNewTransaction(log);
            }
            catch {
                return false;
            }
            return true;            
        }
        
        /// <summary>
        /// The task queues execution implementation block
        /// </summary>
        /// <param name="transactionObjects"></param>
        /// <returns></returns>
        public bool Execute(ConcurrentQueue<object> transactionObjects)
        {
            if (isCancel == true)
                return false;
           
            // get item from the queues for exection 
            // also cast to fit with business object.
            if (transactionObjects.IsEmpty == false)
            {
                int count = transactionObjects.Count;
                //// get all the queues item to add into the bulk insert into database                                   
                if (count >= bulkBindCount)
                {
                    // apply bulk
                }
                else
                {
                    DateTime now = DateTime.Now;
                    TimeSpan span = now.Subtract(this.lastCall);
                    if (span.TotalMilliseconds > (this.recollectPeriod)) // convert to miliisecond
                    {
                        // do nothing
                    }
                    else
                    {
                        return false;
                    }
                }

                if (count > this.bulkBindCount)
                    count = bulkBindCount;
              
                //// get all the queues item to add into the bulk insert into database                   
                IEnumerable<T> logs = GetLoggerList(transactionObjects, count);
                
                try
                {
                    this.bulkInsert.WriteToServer(logs);
                    lastCall = DateTime.Now;
                    logger.AddtoLog("add new records " + count, LoggerLevel.Info);

                    // if(fileLogger != null)
                    //     WriteLogToFile(logs);
                }
                catch (Exception e)
                {
                    //if (fileLogger != null)
                    //    WriteLogToFile(logs);
                    //throw new Exception("Internal error", e);
                }
             }
            
            
            return true;
        }

        private void WriteLogToFile(IEnumerable<T> transactionObjects)
        {
            foreach (var l in transactionObjects)
            {                
                fileLogger.AddtoLogs(l);
            }                            
        }

        public String ToJsonString(T log)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(log);
        }     

        /// <summary>
        /// Get the logs from the log queues that pass from the tasks queues manager.
        /// </summary>
        /// <param name="logQueues"></param>
        /// <returns></returns>
        public IEnumerable<T> GetLoggerList(ConcurrentQueue<object> logQueues, int count)
        {
            if (logQueues.IsEmpty == false)
                for (int i = 0; i < count; i++)
                {
                    object log = null;
                    logQueues.TryDequeue(out log);
                    T l = null;

                    if (attributeValidator != null)
                        l = (T)(attributeValidator.Validate(log));
                    else
                        l = (T)log;

                    yield return l;
                }             
        }

        /// <summary>
        /// Get the logs from the log queues that pass from the tasks queues manager.
        /// </summary>
        /// <param name="logQueues"></param>
        /// <returns></returns>
        public IEnumerable<T> GetLoggerList(ConcurrentQueue<object> logQueues)
        {
            
            int count = logQueues.Count;
            return GetLoggerList(logQueues, count);
        }


        /// <summary>
        /// Dispose from memory
        /// </summary>
        public void  Dispose()
        {
            
            try
            {
                isCancel = true;

                if (this.taskManager.TransactionQueues != null)
                {
                    // get all the queues item to add into the bulk insert into database                   
                    logger.AddtoLog("Starting add db log  at " + DateTime.Now, LoggerLevel.Info);
                    IEnumerable<T> logs = GetLoggerList(this.taskManager.TransactionQueues);
                   
                    if (logs != null)
                    {
                        this.bulkInsert.WriteToServer<T>(logs);
                    }
                    if (logs != null)
                    {
                        try
                        {
                            WriteLogToFile(logs);
                        }
                        catch {
                            logger.AddtoLog("Failed to write logs to log file at " + DateTime.Now, LoggerLevel.Info);
                        }
                    }

                    logger.AddtoLog("Completed add db logs  at " + DateTime.Now, LoggerLevel.Info);
                }
                
                try
                {
                    
                    //fileLogger.Dispose();
                }
                catch(Exception ee)
                {
                    logger.AddtoLog("Failed to collect all add log into file at " + DateTime.Now + " due to - ", ee, LoggerLevel.Error);
                }
            }
            catch (Exception e)
            {
                logger.AddtoLog("Failed to collect all add db log  at " + DateTime.Now + " due to - ", e, LoggerLevel.Error);
            }
 
            try
            {
                this.taskManager.Dispose();
            }
            catch { }             
                                                               
        }
    }
}   
