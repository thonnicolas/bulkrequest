using System;
using log4net;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace Asiacell.ITADLibraries.Utilities
{
    sealed public class Logger
    {
        private readonly ILog logger;

        private string PID(object PID, object message) { return String.Format("PID:{0} - {1}", PID, message); }

        #region Error output format
        public string MessageError(Exception exception)
        {
            StackTrace stackTrace = new StackTrace(exception, true);
            StackFrame frame = stackTrace.GetFrame((stackTrace.FrameCount - 1));
            MethodBase methodBase = frame.GetMethod();    //These two lines are respnsible to find out name of the method
            int Line = frame.GetFileLineNumber(); // Get Line number 
            string methodname = methodBase.Name; //exception.TargetSite.Name;
            return String.Format("Method : {0}, Descript : {1}, Line : {2}", methodname, exception.Message, Line);
        }


        private string MessageError(Exception exception, object pid) { return PID(pid, MessageError(exception)); }
        private string MessageError(Exception exception, string sql) { return String.Format("{0}, SQL : {1}", MessageError(exception), sql); }
        private string MessageError(Exception exception, object pid, string sql) { return PID(pid, MessageError(exception, sql)); }
        #endregion Error output format

        private string GetWorkerID(Type type, object WorkerID)
        {
            return type.ToString() + ", WorkerID : " + WorkerID;
        }

        public Logger(Type classname)
        {
            logger = LogManager.GetLogger(classname);
        }

        public Logger(Type classname, object WorkerID)
        {
            logger = LogManager.GetLogger(GetWorkerID(classname, WorkerID));
        }


        #region Debug
        public void Debug(object pid, object message, Exception exception, out string error)
        {
            error = MessageError(exception, pid);
            logger.Debug(PID(pid, message), exception);
        }

        public void Debug(object message, Exception exception)
        {
            logger.Debug(message, exception);
        }

        public void Debug(object message)
        {
            logger.Debug(message);
        }

        public void Debug(object pid, object message)
        {
            logger.Debug(PID(pid, message));
        }
        #endregion Debug

        #region Fatal
        public void Fatal(object message, Exception exception)
        {
            logger.Fatal(message, exception);
        }

        public void Fatal(object pid, object message, Exception exception)
        {
            logger.Fatal(PID(pid, message), exception);
        }

        public void Fatal(object message)
        {
            logger.Fatal(message);
        }

        public void Fatal(object pid, object message)
        {
            logger.Fatal(PID(pid, message));
        }
        #endregion Fatal

        #region Info
        public void Info(object message, Exception exception)
        {
            logger.Info(message, exception);
        }

        public void Info(object pid, object message, Exception exception)
        {
            logger.Info(PID(pid, message), exception);
        }

        public void Info(object message)
        {

            logger.Info(message);
        }

        public void Info(object pid, object message)
        {
            logger.Info(PID(pid, message));
        }
        #endregion Info

        #region Warm
        public void Warn(object message, Exception exception)
        {
            logger.Warn(message, exception);
        }

        public void Warn(object pid, object message, Exception exception)
        {
            logger.Warn(PID(pid, message), exception);
        }

        public void Warn(object message)
        {
            logger.Warn(message);
        }

        public void Warn(object pid, object message)
        {
            logger.Warn(PID(pid, message));
        }
        #endregion Warm


        #region Error
        public void Error(object message)
        {
            logger.Error(message);
        }

        public void Error(object pid, object message)
        {
            logger.Error(PID(pid, message));
        }

        public void Error(object message, Exception exception)
        {
            logger.Error(message, exception);
        }

        public void Error(object pid, object message, Exception exception)
        {
            logger.Error(PID(pid, message), exception);
        }

        public void ErrorSQL(Exception exception)
        {
            logger.Error(MessageError(exception));
        }

        public void ErrorSQL(Exception exception, out string error)
        {
            error = MessageError(exception);
            logger.Error(error);
        }

        public void ErrorSQL(Exception exception, string SQL)
        {
            logger.Error(MessageError(exception, SQL));
        }

        public void ErrorSQL(Exception exception, string SQL, out string error)
        {
            error = MessageError(exception, SQL);
            logger.Error(error);
        }


        public void ErrorSQL(object pid, Exception exception)
        {
            logger.Error(MessageError(exception, pid));
        }

        public void ErrorSQL(object pid, Exception exception, out string error)
        {
            error = MessageError(exception, pid);
            logger.Error(error);
        }

        public void ErrorSQL(object pid, Exception exception, string SQL)
        {
            logger.Error(MessageError(exception, pid, SQL));
        }

        public void ErrorSQL(object pid, Exception exception, string SQL, out string error)
        {
            error = MessageError(exception, pid, SQL);
            logger.Error(error);
        }

        #endregion Error
    }
}
