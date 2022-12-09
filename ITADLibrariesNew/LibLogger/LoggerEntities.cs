using System;
using System.Collections.Concurrent;
using log4net;
using log4net.Config;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using Asiacell.ITADLibraries_v1.LibSocket;
using Asiacell.ITADLibraries_v1.Utilities;
using System.Net;

namespace Asiacell.ITADLibraries_v1.LibLogger
{
    public class LoggerEntities : IDisposable
    {
        // Singleton instance
        private static volatile LoggerEntities _instance;
        private static object syncRoot = new Object();

        private ConcurrentQueue<LoggerAttribute> logData = null;
        private ConcurrentQueue<String> traLog = null;
        private ILog logger = null;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private Task Addtask = null;

        /// <summary>
        /// Get Server object instance
        /// </summary>
        public static LoggerEntities Instance()
        {
            //get
            //{

            if (_instance == null)
            {
                lock (syncRoot) // thread safe
                {                    
                    if (_instance == null)
                        _instance = new LoggerEntities();
                }
            }

            return _instance;
        }

        public LoggerEntities()
        {
            XmlConfigurator.Configure();
            logger = LogManager.GetLogger("ITMDW-Log");
            logData = new ConcurrentQueue<LoggerAttribute>();
            AddToLogTask();
        }

        public LoggerEntities(ConcurrentQueue<string> tracLog)
        {
            traLog = tracLog;
            XmlConfigurator.Configure();
            logger = LogManager.GetLogger("ITMDW-Log");
            logData = new ConcurrentQueue<LoggerAttribute>();
            AddToLogTask();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProcessID"></param>
        /// <param name="ERRORCODE"></param>
        /// <param name="ClientIP"></param>
        /// <param name="UserName"></param>
        /// <param name="CommandID"></param>
        /// <param name="Command"></param>
        /// <param name="Submited_On"></param>
        /// <param name="Responded_On"></param>
        /// <param name="Result"></param>
        /// <param name="loglevel"></param>
        public void AddtoLog(String ProcessID, int ERRORCODE, String CommandID,
            String Command, DateTime Submited_On, DateTime Responded_On, String Result, LoggerLevel loglevel, ref StateObject state)
        {
            string MethodName = string.Empty;
            try
            {
                ///To get called method-name
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase methodBase = stackFrame.GetMethod();
                MethodName = methodBase.Name;
            }
            catch { }
            string ClientIP = ((IPEndPoint)state.ClientSocket.RemoteEndPoint).Address.ToString();

            LoggerAttribute gendataLog = new LoggerAttribute(MethodName, ProcessID, ERRORCODE, ClientIP, state.UserName, CommandID, Command, Submited_On, Responded_On, Result, loglevel);

            logData.Enqueue(gendataLog);
            if (state != null)
                state.SetToLog(GenerateLogMessage(gendataLog));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProcessID"></param>
        /// <param name="ERRORCODE"></param>
        /// <param name="ClientIP"></param>
        /// <param name="UserName"></param>
        /// <param name="CommandID"></param>
        /// <param name="Command"></param>
        /// <param name="Submited_On"></param>
        /// <param name="Responded_On"></param>
        /// <param name="Result"></param>
        /// <param name="loglevel"></param>
        public void AddtoLog(String ProcessID, int ERRORCODE, String ClientIP, String UserName, String CommandID,
            String Command, DateTime Submited_On, DateTime Responded_On, String Result, LoggerLevel loglevel)
        {
            string MethodName = string.Empty;
            try
            {
                ///To get called method-name
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase methodBase = stackFrame.GetMethod();
                MethodName = methodBase.Name;
            }
            catch { }

            LoggerAttribute gendataLog = new LoggerAttribute(MethodName, ProcessID, ERRORCODE, ClientIP, UserName, CommandID, Command, Submited_On, Responded_On, Result, loglevel);

            logData.Enqueue(gendataLog);
        }

        /// <summary>
        /// Add to log items to logger data
        /// </summary>
        /// <param name="ProcessID"></param>
        /// <param name="ERRORCODE"></param>
        /// <param name="ClientIP"></param>
        /// <param name="UserName"></param>
        /// <param name="CommandID"></param>
        /// <param name="Command"></param>
        /// <param name="Submited_On"></param>
        /// <param name="Responded_On"></param>
        /// <param name="loglevel"></param>
        /// <param name="exception"></param>
        public void AddtoLog(String ProcessID, int ERRORCODE, String CommandID,
            String Command, DateTime Submited_On, DateTime Responded_On, LoggerLevel loglevel, Exception exception, ref StateObject state)
        {
            ///To get called method-name with Line number from exception
            string methodName = string.Empty;
            int Line = 0;
            try
            {
                StackTrace stackTrace = new StackTrace(exception, true);
                StackFrame stackFrame = stackTrace.GetFrame((stackTrace.FrameCount - 1));
                Line = stackFrame.GetFileLineNumber(); // Get Line number 
                MethodBase methodBase = stackFrame.GetMethod();
                methodName = methodBase.Name;
            }
            catch { }
            //logData.Enqueue(new LoggerAttribute(methodName, ProcessID, ERRORCODE, ClientIP, UserName, CommandID, Command, Submited_On, Responded_On, exception.Message + "Line:" + Line, loglevel));

            string ClientIP = ((IPEndPoint)state.ClientSocket.RemoteEndPoint).Address.ToString();

            LoggerAttribute gendataLog = new LoggerAttribute(methodName, ProcessID, ERRORCODE, ClientIP, state.UserName, CommandID, Command, Submited_On, Responded_On, exception.Message + "Line:" + Line, loglevel);

            logData.Enqueue(gendataLog);
            if (state != null)
                state.SetToLog(GenerateLogMessage(gendataLog));
        }

        public void AddtoLog(Exception ex, LoggerLevel logLevel)
        {
            string methodName = string.Empty;
            int Line = 0;
            try
            {
                StackTrace stackTrace = new StackTrace(ex, true);
                StackFrame stackFrame = stackTrace.GetFrame((stackTrace.FrameCount - 1));
                Line = stackFrame.GetFileLineNumber(); // Get Line number 
                MethodBase methodBase = stackFrame.GetMethod();
                methodName = methodBase.Name;
            }
            catch { }
            logData.Enqueue(new LoggerAttribute(methodName, ex.Message + ", Line : " + Line, logLevel));
        }


        public void AddtoLog(Exception ex, LoggerLevel logLevel, ref StateObject state)
        {
            string methodName = string.Empty;
            int Line = 0;
            try
            {
                StackTrace stackTrace = new StackTrace(ex, true);
                StackFrame stackFrame = stackTrace.GetFrame((stackTrace.FrameCount - 1));
                Line = stackFrame.GetFileLineNumber(); // Get Line number 
                MethodBase methodBase = stackFrame.GetMethod();
                methodName = methodBase.Name;
            }
            catch { }

            LoggerAttribute getlogdata = new LoggerAttribute(methodName, ex.Message + ", Line : " + Line, logLevel);
            logData.Enqueue(getlogdata);
            if (state != null)
                state.SetToLog(GenerateLogMessage(getlogdata));
        }

        //public void AddtoLog(String PID, Exception ex, LoggerLevel logLevel)
        //{
        //    string methodName = string.Empty;
        //    int Line = 0;
        //    try
        //    {
        //        StackTrace stackTrace = new StackTrace(ex, true);
        //        StackFrame stackFrame = stackTrace.GetFrame((stackTrace.FrameCount - 1));
        //        Line = stackFrame.GetFileLineNumber(); // Get Line number 
        //        MethodBase methodBase = stackFrame.GetMethod();
        //        methodName = methodBase.Name;
        //    }
        //    catch { }
        //    logData.Enqueue(new LoggerAttribute(PID, methodName, ex.Message + ", Line : " + Line, logLevel));
        //}

        public void AddtoLog(String PID, Exception ex, LoggerLevel logLevel, ref StateObject state)
        {
            string methodName = string.Empty;
            int Line = 0;
            try
            {
                StackTrace stackTrace = new StackTrace(ex, true);
                StackFrame stackFrame = stackTrace.GetFrame((stackTrace.FrameCount - 1));
                Line = stackFrame.GetFileLineNumber(); // Get Line number 
                MethodBase methodBase = stackFrame.GetMethod();
                methodName = methodBase.Name;
            }
            catch { }

            LoggerAttribute getlogdata = new LoggerAttribute(PID, methodName, ex.Message + ", Line : " + Line, logLevel);

            logData.Enqueue(getlogdata);
            if (state != null)
                state.SetToLog(GenerateLogMessage(getlogdata));
        }

        public void AddtoLog(String PID, Exception ex, LoggerLevel logLevel)
        {
            string methodName = string.Empty;
            int Line = 0;
            try
            {
                StackTrace stackTrace = new StackTrace(ex, true);
                StackFrame stackFrame = stackTrace.GetFrame((stackTrace.FrameCount - 1));
                Line = stackFrame.GetFileLineNumber(); // Get Line number 
                MethodBase methodBase = stackFrame.GetMethod();
                methodName = methodBase.Name;
            }
            catch { }

            LoggerAttribute getlogdata = new LoggerAttribute(PID, methodName, ex.Message + ", Line : " + Line, logLevel);
            logData.Enqueue(getlogdata);
            
        }

        public void AddtoLog(string Message, LoggerLevel logLevel)
        {
            string MethodName = string.Empty;
            try
            {
                ///To get called method-name
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase methodBase = stackFrame.GetMethod();
                MethodName = methodBase.Name;
            }
            catch { }
            logData.Enqueue(new LoggerAttribute(MethodName, Message, logLevel));
        }

        public void AddtoLog(string Message, LoggerLevel logLevel, ref StateObject state)
        {
            string MethodName = string.Empty;
            try
            {
                ///To get called method-name
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase methodBase = stackFrame.GetMethod();
                MethodName = methodBase.Name;
            }
            catch { }

            LoggerAttribute getlogdata = new LoggerAttribute(MethodName, Message, logLevel);
            logData.Enqueue(getlogdata);
            if (state != null)
                state.SetToLog(GenerateLogMessage(getlogdata));
        }

        public void AddtoLog(string PID, string Message, LoggerLevel logLevel)
        {
            string MethodName = string.Empty;
            try
            {
                ///To get called method-name
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase methodBase = stackFrame.GetMethod();
                MethodName = methodBase.Name;
            }
            catch { }
            logData.Enqueue(new LoggerAttribute(PID, MethodName, Message, logLevel));
        }

        public void AddtoLog(string PID, string Message,Exception ex,  LoggerLevel logLevel)
        {
            Int32 Line=0;
            string methodName = string.Empty;
            try
            {
                ///To get called method-name
                StackTrace stackTrace = new StackTrace(ex, true);
                StackFrame stackFrame = stackTrace.GetFrame((stackTrace.FrameCount - 1));
                Line = stackFrame.GetFileLineNumber(); // Get Line number 
                MethodBase methodBase = stackFrame.GetMethod();
                methodName = methodBase.Name;
            }
            catch { }
            //LoggerAttribute getlogdata = new LoggerAttribute(PID, methodName, Message + ", Line : " + Line + ":" + ex.InnerException.Message, logLevel);
            LoggerAttribute getlogdata = new LoggerAttribute(PID, methodName, Message + ", Line : " + Line , logLevel);
            logData.Enqueue(getlogdata);

        }

        public void AddtoLog(string PID, string Message, DateTime Submited_On, DateTime Responded_On, LoggerLevel logLevel)
        {
            string MethodName = string.Empty;
            try
            {
                ///To get called method-name
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase methodBase = stackFrame.GetMethod();
                MethodName = methodBase.Name;
            }
            catch { }
            logData.Enqueue(new LoggerAttribute(PID, MethodName, Message, Submited_On, Responded_On, logLevel));
        }


        public void AddtoLog(string PID, string Message, LoggerLevel logLevel, StateObject state)
        {
            string MethodName = string.Empty;
            try
            {
                ///To get called method-name
                StackTrace stackTrace = new StackTrace();
                StackFrame stackFrame = stackTrace.GetFrame(1);
                MethodBase methodBase = stackFrame.GetMethod();
                MethodName = methodBase.Name;
            }
            catch { }

            LoggerAttribute getlogdata = new LoggerAttribute(PID, MethodName, Message, logLevel);
            logData.Enqueue(getlogdata);
            if (state != null)
                state.SetToLog(GenerateLogMessage(getlogdata));
        }


        /// <summary>
        /// Right to Log file
        /// </summary>
        private void AddToLogTask()
        {

            CancellationToken token = tokenSource.Token;

            Addtask = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (logData.IsEmpty && token.IsCancellationRequested)
                        break;
                    if (!logData.IsEmpty)
                    {
                        LoggerAttribute att = null;
                        if (logData.TryDequeue(out att))
                        {
                            string message = GenerateLogMessage(att);
                            if (traLog != null) traLog.Enqueue(message);
                            switch (att.loglevl)
                            {
                                case LoggerLevel.Error:
                                    logger.Error(message);
                                    break;
                                case LoggerLevel.Fatal:
                                    logger.Fatal(message);
                                    break;
                                case LoggerLevel.Info:
                                    logger.Info(message);
                                    break;
                            }
                        }
                    }
                    else
                        Addtask.Wait(100);
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// <UserName><ProcessID:XXXXXXXXXXXXXXXXXX><ERRORCODE><Client-IP><Request_On - Respond_On><CommandID:Command><Result><Module-Name>
        /// </summary>
        /// <param name="att"></param>
        /// <returns></returns>
        public String GenerateLogMessage(LoggerAttribute att)
        {
            string datalog = string.Empty;

            if (att.IsOnlyResult)
                if (string.IsNullOrWhiteSpace(att.ProcessID))
                    datalog = String.Format("<{0}><{1}><{2}>", att.ModuleName, att.Result, att.Sumited_On.ToString(Functions.dateFormateLong));
                else
                    if (att.Sumited_On == att.Responded_On)
                        datalog = String.Format("<{0}><{1}><{2}><{3}>", att.ProcessID, att.ModuleName, att.Result, att.Sumited_On.ToString(Functions.dateFormateLong));
                    else
                        datalog = String.Format("<{0}><{1}><{2}><{3} - {4}>", att.ProcessID, att.ModuleName, att.Result, att.Sumited_On.ToString(Functions.dateFormateLong), att.Responded_On.ToString(Functions.dateFormateLong));
            else
                datalog = String.Format("<{0}><{1}><{2}><{3}><{4} - {5}><CommandID={6}:Command={7}><{8}><{9}>", att.UserName, att.ProcessID,
                    att.ERRORCODE, att.Client_IP, att.Sumited_On.ToString(Functions.dateFormateLong), att.Responded_On.ToString(Functions.dateFormateLong), att.CommandID, att.Command, att.Result, att.ModuleName);

            return datalog;
        }

        public void Dispose()
        {
            tokenSource.Cancel();
            Addtask.Wait();
        }
    }
}
