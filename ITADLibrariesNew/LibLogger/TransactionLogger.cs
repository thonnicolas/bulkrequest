using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries_v1.Utilities;
using System.Collections.Concurrent;
using System.IO;

namespace Asiacell.ITADLibraries_v1.LibLogger
{
    public class TransactionLogger<T> : ITaskExecutor, IDisposable where T : class
    {
        private string logFilePath;
        private LoggerEntities logger = null;
        private TaskExecutorManagerFile taskManager;
        private bool isCancel = false;
        private IAttributeValidator attributeValidator = null;

        public TransactionLogger(string logFilePath, LoggerEntities logger, IAttributeValidator attributeValidator)
        {
            this.logger = logger;
            this.attributeValidator = null;
            this.logFilePath = logFilePath;
            taskManager = new TaskExecutorManagerFile(this);
            this.attributeValidator = attributeValidator;
        }
        /// <summary>
        /// Add to task queues manager
        /// </summary>
        /// <param name="log"></param>
        public bool AddtoLogs(T log)
        {            
            if (isCancel == true)
                return false;
            this.taskManager.AddNewTransaction(log);
                return true;          
        }

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
                IEnumerable<T> logs = GetLoggerList(transactionObjects, count);
                
                try
                {
                    WriteLogToFile(logs);
                }
                catch (Exception e)
                {

                    throw new Exception("Internal error", e);
                }
            }
            
            return true;
        }

        private void WriteLogToFile(IEnumerable<T> transactionObjects)
        {
                       
            lock (this)            
            {
                string logFilePath = this.logFilePath + (DateTime.Now.ToString("yyyyMMddHH")) + ".log";
                using (StreamWriter f = File.AppendText(logFilePath))
                {
                    foreach (var l in transactionObjects)
                    {                        
                        f.WriteLine(ToJsonString(l));                        
                    }
                }            
            }
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
            //int count = logQueues.Count;
            int count = logQueues.Count;
            return GetLoggerList(logQueues, count);
        }

        public String ToJsonString(T log)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(log);
        }

        public void Dispose()
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
                        this.WriteLogToFile(logs);
                    }

                    logger.AddtoLog("Completed add db logs  at " + DateTime.Now, LoggerLevel.Info);

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
