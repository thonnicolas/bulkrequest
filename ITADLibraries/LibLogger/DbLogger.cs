using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.Utilities;
using System.Collections.Concurrent;
using Oracle.DataAccess.Client;
using Oracle.DataAccess;
using Asiacell.ITADLibraries.LibDatabase;
using System.Data;
using System.IO;
using System.ComponentModel;
using log4net.Appender;
using log4net.Layout;
using log4net;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Asiacell.ITADLibraries.LibLogger
{
    [Serializable]
    public class MyLogs<T> : ICloneable where T:class
    {
        public IEnumerable<T> logs;

        public object Clone()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, this);
            ms.Position = 0;
            object obj = bf.Deserialize(ms);
            ms.Close();
            return obj;
        }
    }
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
        private static object syncLock = new object();
        private TransactionLogger<T> fileLogger = null;
        private long countForTest = 0;
        private long rotateSize = 10; // MB
        private MissingTransactionLogger<T> tranLogger;
        private int DbRequestBulkInsertTimeout = 0;
 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLogger(DBConnection db, LoggerEntities logger, int bulkBindCount, string logFilePath, string logFile, int rotateSize, int recollectPeriod, IAttributeValidator attributeValidator, MissingTransactionLogger<T> tranLogger)
        {

            Initialize(db, logger, bulkBindCount, logFilePath, logFile, rotateSize, recollectPeriod, "tbl_logger", false, null, attributeValidator, tranLogger);
   
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLogger(DBConnection db, LoggerEntities logger, int bulkBindCount, string logFilePath, string logFile, int rotateSize, int recollectPeriod, string table_name, bool useBulkCopy, IAttributeValidator attributeValidator, MissingTransactionLogger<T> tranLogger)
        {
            Initialize(db, logger, bulkBindCount, logFilePath, logFile, rotateSize, recollectPeriod, table_name, useBulkCopy, null, attributeValidator, tranLogger);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLogger(DBConnection db, LoggerEntities logger, int bulkBindCount, string logFilePath, string logFile, int rotateSize, int recollectPeriod, string table_name, bool useBulkCopy, IAttributeValidator attributeValidator, MissingTransactionLogger<T> tranLogger, int DbRequestBulkInsertTimeout = 0)
        { 
            Initialize(db, logger, bulkBindCount, logFilePath, logFile, rotateSize, recollectPeriod, table_name, useBulkCopy, null, attributeValidator, tranLogger, DbRequestBulkInsertTimeout);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLogger(DBConnection db, LoggerEntities logger, int bulkBindCount, string logFilePath, string logFile, int rotateSize, int recollectPeriod, TaskExecutorManager taskManager, IAttributeValidator attributeValidator, MissingTransactionLogger<T> tranLogger)
        {
            Initialize(db, logger, bulkBindCount, logFilePath, logFile, rotateSize, recollectPeriod, "tbl_logger", false, taskManager, attributeValidator, tranLogger);            
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
        private void Initialize(DBConnection db, LoggerEntities logger, int bulkBindCount, string logFilePath, string logFile, int rotateSize, int recollectPeriod,
            string table_name, bool useBulkCopy, TaskExecutorManager taskExectorManager, IAttributeValidator attributeValidator,   MissingTransactionLogger<T> tranLogger, int DbRequestBulkInsertTimeout = 0)
        {
            this.bulkBindCount = bulkBindCount;
            this.bulkInsert = new DBDataTransaction(table_name, db, bulkBindCount, useBulkCopy, logger, DbRequestBulkInsertTimeout);
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
            //this.fileLogger = fileLogger;

            this.rotateSize = rotateSize;
            this.tranLogger = tranLogger;           
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

        public IEnumerable<T> DeepCopy(IEnumerable<T> objectToCopy)
        {
            IEnumerable<T> copy;
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, objectToCopy);
                ms.Position = 0;
                copy = (IEnumerable<T>)serializer.ReadObject(ms);
            }
            return copy;
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
                        //this.lastCall = now;
                        // do nothing
                    }
                    else
                    {
                        return false;
                    }
                }

                if (count > this.bulkBindCount)
                    count = this.bulkBindCount;

                this.countForTest += count;
                //// get all the queues item to add into the bulk insert into database                   
                List<T> logs = GetLoggerList(transactionObjects, count);
                try
                {
                    this.lastCall = DateTime.Now;
                    //if (this.countForTest >= 20000) // for test purpose
                    //    throw new Exception("error bulk sql");
                    //else // for test purpose
                    //    this.bulkInsert.WriteToServer(logs);

                    this.bulkInsert.WriteToServer(logs);
                    logger.AddtoLog("add new records " + count, LoggerLevel.Info);
                    //logs.Clear();
                    //slogs = null;
                }
                catch (Exception e)
                {
                    // any error to log to missing log files
                    if (tranLogger != null)
                    {
                        try
                        {
                            /*T[] list = new T[logs.Count];
                            logs.CopyTo(list);
                            */
                            WriteLogToFile(logs);
                            logger.AddtoLog("begin add missing records, records " + logs.Count, e, LoggerLevel.Info);
                        }
                        catch (Exception ew)
                        {
                            logger.AddtoLog("Failed begin add missing records, records " + logs.Count, ew, LoggerLevel.Error);
                        }
                    }
                    else
                    {
                        //throw new Exception("Internal error", e);
                        logger.AddtoLog("Failed to add missing records " + count, e, LoggerLevel.Info);
                    }                    
                }
            }
            else
            {
                this.lastCall = DateTime.Now;
            }

            return true;
             
        }

        private void WriteLogToFile(IEnumerable<T> transactionObjects)
        {
            foreach (var l in transactionObjects)
            {
                try
                {
                    //tranLogger.Info(ToJsonString(l));
                    tranLogger.AddtoLogs((T)l);
                }
                catch (Exception ex)
                {
                    logger.AddtoLog("Error add missing records with: ", ex , LoggerLevel.Fatal);
                }
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
        //public IEnumerable<T> GetLoggerList(ConcurrentQueue<object> logQueues, int count)
        //{
        //    if (logQueues.IsEmpty == false)
        //        for (int i = 0; i < count; i++)
        //        {
        //            object log = null;
        //            logQueues.TryDequeue(out log);
        //            T l = null;

        //            if (attributeValidator != null)
        //                l = (T)(attributeValidator.Validate(log));
        //            else
        //                l = (T)log;

        //            yield return l;
        //        }             
        //}

        public List<T> GetLoggerList(ConcurrentQueue<object> logQueues, int count)
        {
            List<T> myList = new List<T>();
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
                    myList.Add(l);
                    //yield return l;
                }
            return myList;
        }

        /// <summary>
        /// Get the logs from the log queues that pass from the tasks queues manager.
        /// </summary>
        /// <param name="logQueues"></param>
        /// <returns></returns>
        public List<T> GetLoggerList(ConcurrentQueue<object> logQueues)
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
                    //logger.AddtoLog("Starting add db log  at " + DateTime.Now, LoggerLevel.Info);
                    //IEnumerable<T> logs = GetLoggerList(this.taskManager.TransactionQueues);

                    List<T> logs = GetLoggerList(this.taskManager.TransactionQueues);
                   

                    try{
                        if (logs != null && logs.Count > 0)
                        {
                            this.bulkInsert.WriteToServer<T>(logs);
                        }
                    }
                     catch (Exception e)
                    {

                        if (tranLogger != null)
                        {
                            try
                            {
                                /*
                                T[] list = new T[logs.Count];
                                logs.CopyTo(list);
                                */
                                WriteLogToFile(logs);
                                logger.AddtoLog("begin add missing records, records " + logs.Count, e, LoggerLevel.Info);
                            }
                            catch (Exception ew)
                            {
                                logger.AddtoLog("Failed begin add missing records, records " + logs.Count, ew, LoggerLevel.Error);
                            }
                            
                        }
                        //throw new Exception("Internal error", e);                        

                    }
                    logger.AddtoLog("Completed add db logs  at " + DateTime.Now, LoggerLevel.Info);
                }

                try
                {

                    tranLogger.Dispose();
                }
                catch (Exception ee)
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
