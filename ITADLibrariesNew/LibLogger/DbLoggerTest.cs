using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Asiacell.ITADLibraries_v1.Utilities;
using Asiacell.ITADLibraries_v1.LibDatabase;

namespace Asiacell.ITADLibraries_v1.LibLogger
{
    public class DbLoggerTest  : ITaskExecutor, IDisposable
    {
        private int bulkBindCount;
        private DBDataTransaction bulkInsert;
        private TaskExecutorManager taskManager;
        private string logFilePath;
        private DateTime lastCall;
        private int recollectPeriod = 10000; // millisecond
        private bool isCancel = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLoggerTest(DBConnection db, int bulkBindCount, string logFilePath, int recollectPeriod, string tableName)
        {
            this.bulkBindCount = bulkBindCount;
            this.bulkInsert = new DBDataTransaction(tableName, db, bulkBindCount);
            this.recollectPeriod = recollectPeriod * 1000;
            this.taskManager = new TaskExecutorManager(this);
            this.logFilePath = logFilePath;
            this.lastCall = DateTime.Now;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLoggerTest(DBConnection db, int bulkBindCount, string logFilePath, int recollectPeriod)
        {
            this.bulkBindCount = bulkBindCount;
            this.bulkInsert = new DBDataTransaction("tbl_logger", db, bulkBindCount);
            this.recollectPeriod = recollectPeriod * 1000;
            this.taskManager = new TaskExecutorManager(this);
            this.logFilePath = logFilePath;
            this.lastCall = DateTime.Now;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bulkBindCount"></param>
        public DbLoggerTest(DBConnection db, int bulkBindCount, string logFilePath, int recollectPeriod, TaskExecutorManager taskManager)
        {
            this.bulkBindCount = bulkBindCount;
            this.bulkInsert = new DBDataTransaction("tbl_logger", db, bulkBindCount);
            this.taskManager = taskManager;
            this.taskManager.SetExecutor(this);
            this.recollectPeriod = recollectPeriod * 1000;
            this.logFilePath = logFilePath;
            this.lastCall = DateTime.Now;
        }

        /// <summary>
        /// Add to task queues manager
        /// </summary>
        /// <param name="log"></param>
        public void AddtoLogs(LoggerDBAttributeTest log)
        {
            try
            {
                if (isCancel == false)
                    this.taskManager.AddNewTransaction(log);
            }
            catch (Exception e)
            {
                throw new Exception("Cannot add new transaction", e);
            }
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
                //IEnumerable<LoggerDBAttributeTest> logs = GetLoggerList(transactionObjects);
                if (count >= bulkBindCount)
                {
                    // apply bulid
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
                IEnumerable<LoggerDBAttributeTest> logs = GetLoggerList(transactionObjects, count);
                try
                {                    
                    this.bulkInsert.WriteToServer(logs);
                    lastCall = DateTime.Now;
                    WriteLogToFile(logs);
                }
                catch (Exception e)
                {
                    WriteLogToFile(logs);
                }
            }
            
            return true;
        }

        private void WriteLogToFile(IEnumerable<LoggerDBAttributeTest> transactionObjects)
        {

            //string logFilePath = this.logFilePath + (DateTime.Now.ToString("yyyyMMddHH")) + "00.log";
            //using (StreamWriter f = File.AppendText(logFilePath))
            //{
            //    foreach (var l in transactionObjects)
            //    {
            //        f.WriteLine(ToJsonString(l));
            //        f.WriteLine("--BLOCK--");
            //    }
            //}
        }

        public String ToJsonString(LoggerDBAttributeTest log)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(log);
        }     

        /// <summary>
        /// Get the logs from the log queues that pass from the tasks queues manager.
        /// </summary>
        /// <param name="logQueues"></param>
        /// <returns></returns>
        public IEnumerable<LoggerDBAttributeTest> GetLoggerList(ConcurrentQueue<object> logQueues, int count)
        {
            if (logQueues.IsEmpty == false)
                for (int i = 0; i < count; i++)
                {
                    object log = null;
                    logQueues.TryDequeue(out log);
                    yield return (LoggerDBAttributeTest)log;
                }
        }

        /// <summary>
        /// Get the logs from the log queues that pass from the tasks queues manager.
        /// </summary>
        /// <param name="logQueues"></param>
        /// <returns></returns>
        public IEnumerable<LoggerDBAttributeTest> GetLoggerList(ConcurrentQueue<object> logQueues)
        {
            //int count = logQueues.Count;
            int count = logQueues.Count;
            return GetLoggerList(logQueues, count);
        }


        /// <summary>
        /// Dispose from memory
        /// </summary>
        public void  Dispose()
        {
            isCancel = false;
            
            try
            {
                if (this.taskManager.TransactionQueues.IsEmpty == false)
                {

                    IEnumerable<LoggerDBAttributeTest> logs = GetLoggerList(this.taskManager.TransactionQueues);
                    this.bulkInsert.WriteToServer(logs);
                }

                this.taskManager.Dispose();
            }
            catch{};
                            
        }
    }
}
