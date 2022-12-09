using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.Utilities;
using log4net;
using log4net.Appender;
using log4net.Layout;
using System.Collections.Concurrent;

namespace Asiacell.ITADLibraries.LibLogger
{
    public class MissingTransactionLogger<T> : ITaskExecutor, IDisposable where T : class
    {
        private string logFilePath;
        private LoggerEntities logger = null;
        private TaskExecutorManagerFile taskManager;
        private bool isCancel = false;
        private IAttributeValidator attributeValidator = null;
        private long rotateSize = 10; // MB
        //private ILog tranLogger;
        private readonly ILog tranLogger = LogManager.GetLogger(typeof(TransactionLogger<T>));
        private static object syncLock = new object();

        public void AddAppender2(ILog log, IAppender appender)
        {
            // ILog log = LogManager.GetLogger(loggerName);
            log4net.Repository.Hierarchy.Logger l = (log4net.Repository.Hierarchy.Logger)log.Logger;

            l.AddAppender(appender);
        }

        // Create a new file appender
        public IAppender CreateFileAppender(string name, string fileName)
        {
            
            RollingFileAppender appender = new
                RollingFileAppender();
            appender.Name = name;
            appender.File = fileName;
            appender.AppendToFile = true;            
 
            appender.MaxSizeRollBackups = 1000;
            appender.DatePattern = "yyyy.MM.dd'.log'";
            appender.StaticLogFileName = false;
            //appender.RollingStyle = RollingFileAppender.RollingMode.Size;
            appender.MaximumFileSize = this.rotateSize + "MB";
            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = "%m%n";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }

        public MissingTransactionLogger(string logFilePath, LoggerEntities logger, IAttributeValidator attributeValidator, long rotateSize = 10, string logFile = "translogs.log")
        {
            this.logger = logger;
            this.attributeValidator = null;
            this.logFilePath = logFilePath;
            taskManager = new TaskExecutorManagerFile(this);
            this.attributeValidator = attributeValidator;
            this.rotateSize = rotateSize;
            
            //this.tranLogger = LogManager.GetLogger("TransactionLogFile");
            if (logFilePath.EndsWith("\\"))
            {
                AddAppender2(tranLogger, CreateFileAppender("MissingTransactionLogFile", logFilePath + logFile));
            }
            else
            {
                AddAppender2(tranLogger, CreateFileAppender("MissingTransactionLogFile", logFilePath + "\\" + logFile));
            }
            
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
            catch
            {
                return false;
            }
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
                    logger.AddtoLog("Error write transaction to file", e, LoggerLevel.Fatal);
                    //throw new Exception("Internal error", e);
                }
            }
           
            return true;
        }

        private void WriteLogToFile(IEnumerable<T> transactionObjects)
        {
            foreach (var l in transactionObjects)
            {
                tranLogger.Info(ToJsonString(l));
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
                    l = (T)log;

                    /*
                    if (attributeValidator != null)
                        l = (T)(attributeValidator.Validate(log));
                    else
                        l = (T)log;
                    */

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
                    logger.AddtoLog("Starting add file log  at " + DateTime.Now, LoggerLevel.Info);
                    IEnumerable<T> logs = GetLoggerList(this.taskManager.TransactionQueues);

                    if (logs != null)
                    {
                        this.WriteLogToFile(logs);
                    }

                    logger.AddtoLog("Completed add file logs  at " + DateTime.Now, LoggerLevel.Info);

                }
            }
            catch (Exception e)
            {
                logger.AddtoLog("Failed to collect all add file log  at " + DateTime.Now + " due to - ", e, LoggerLevel.Error);
            }

            try
            {
                this.taskManager.Dispose();

            }
            catch { }             
        }
    }
}
