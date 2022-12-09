using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Types;
using Oracle.ManagedDataAccess.Client;
using System.Threading;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibraries.Utilities;
using System.Data;
using System.Timers;
using System.Threading.Tasks;
using Asiacell.ITADLibraries.Utilities_v1;

namespace Asiacell.ITADLibraries.LibDatabase
{
    public sealed class DBManagedDriverConnection : IDisposable
    {
        private readonly LoggerEntities logger;

        private OracleConnection conn = null;

        private ManualResetEvent IsDown = new ManualResetEvent(false);

        private string ConnectionString = string.Empty;
        private string DBName;
        private string DBVersion;
        private string DBUserID;

        private DateTime DonwnTime;
        private DateTime UpTime;

        private Thread Connection_Recovery = null;
        private bool IsDisposed = false;
        private bool IsError = false;
        private bool IsentNotice = false;

        /// <summary>
        /// Created connection without Encryption
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="logger"></param>
        public DBManagedDriverConnection(string ConnectionString, LoggerEntities logger)
        {
            if (ConnectionString.ToLower().Replace(" ","").Contains("password=") && ConnectionString.ToLower().Contains("user id="))
            {
                this.ConnectionString = ConnectionString;
            }
            else
            {
                this.ConnectionString = (new Decryption("ITAD_AsiaCe11_for!tad", logger)).DecryptTripleDES(ConnectionString);
            }
            //this.ConnectionString = ConnectionString;
            IsDisposed = false;
            this.logger = logger;
        }

        /// <summary>
        /// Create connetion with Encryption 
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="Keys"></param>
        /// <param name="logger"></param>
        public DBManagedDriverConnection(string ConnectionString, string Keys, LoggerEntities logger)
        {

            this.ConnectionString = (new Decryption(Keys,logger)).DecryptTripleDES(ConnectionString);
            IsDisposed = false;
            this.logger = logger;

        }

        /// <summary>
        /// Start DB connection without failover event,
        /// Recommanded : use in web application
        /// </summary>
        /// <returns></returns>
        public bool StartupConnectionWithoutFailover()
        {

            bool IsConnected = false;
            try
            {
                if (conn != null)
                {
                    conn.Dispose();
                    conn = null;
                }

                logger.AddtoLog("Initializing database connection.....", LoggerLevel.Info);
                conn = new OracleConnection(ConnectionString);
                conn.Open();

                if (conn.State == ConnectionState.Open)
                {
                    DBUserID = GetUserID();
                    logger.AddtoLog("Connect to database is successful...", LoggerLevel.Info);
                    DBName = conn.DatabaseName;
                    DBVersion = conn.ServerVersion;

                    logger.AddtoLog("Database Name : " + DBName, LoggerLevel.Info);
                    logger.AddtoLog("Database Version : " + DBVersion, LoggerLevel.Info);
                    logger.AddtoLog("Datababase UserID : " + DBUserID, LoggerLevel.Info);
                    IsConnected = true;
                    UpTime = DateTime.Now;

                }


            }
            catch (OracleException ex)
            {
                if (conn != null)
                    conn.Close();
                logger.AddtoLog(ex, LoggerLevel.Error);
            }
            catch (Exception ex)
            {
                if(conn != null)
                    conn.Close();
                logger.AddtoLog(ex, LoggerLevel.Error);
            }

            return IsConnected;
        }

        /// <summary>
        /// Start up DB connection with failover event. This event is auto recovery connection
        /// </summary>
        /// <returns></returns>
        public bool StartupConnection()
        {
            //lock (this)
            //{
            bool IsConnected = false;
            if (StartupConnectionWithoutFailover())
            {
                IsConnected = true;
                //conn.Failover += new OracleFailoverEventHandler(OnFailover);
                Connection_Recovery = new Thread(AutoConnectionRecovery);
                Connection_Recovery.Start();
            }

            return IsConnected;
            //}
        }

        /// <summary>
        /// Thread to check db to connection
        /// </summary>
        private void AutoConnectionRecovery()
        {

            while (!IsDisposed)
            {
                try
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));

                    if (!IsDisposed)
                    {
                        if (!IsConnected())
                        {
                            logger.AddtoLog("Re-connect to database", LoggerLevel.Info);
                            ReConnectDatabase();
                            //IsDown.WaitOne();
                        }
                    }
                }
                catch (OracleException ex)
                {

                    logger.AddtoLog(ex, LoggerLevel.Error);
                }
                catch (Exception ex)
                {

                    logger.AddtoLog("The database connection is Error : " + ex.Message, LoggerLevel.Error);
                }
            }
        }


        private void ReConnectDatabase()
        {
            //Task.Factory.StartNew(() =>
            //{                
                while (!(bool)IsDisposed)
                {                    
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    if (StartupConnectionWithoutFailover())
                    {
                        logger.AddtoLog("Successfully Re-connect to database", LoggerLevel.Fatal);
                        break;
                    }
                }
                //IsDown.Set();
                //IsDown.Reset();
            //}, TaskCreationOptions.None);
        }


        /// <summary>
        /// To query for checking connection
        /// </summary>
        /// <returns></returns>
        private bool IsConnected()
        {
            bool isConnected = false;
            try
            {
                //logger.AddtoLog("Check database connection....", LoggerLevel.Info);
                this.ExecuteScalar("select sysdate from dual");
                //logger.AddtoLog("The database connection is OK", LoggerLevel.Info);
                isConnected = true;
            }
            catch (OracleException ex)
            {
                DonwnTime = DateTime.Now;

                logger.AddtoLog("The database connection is Error : " + ex.Message, LoggerLevel.Error);

                ///For e-mail notification
                if (!IsentNotice)
                {
                    logger.AddtoLog(ex, LoggerLevel.Fatal);
                    IsentNotice = true;
                }

            }
            catch (Exception ex)
            {
                DonwnTime = DateTime.Now;

                logger.AddtoLog("The database connection is Error : " + ex.Message, LoggerLevel.Error);

                ///For e-mail notification
                if (!IsentNotice)
                {
                    logger.AddtoLog(ex, LoggerLevel.Fatal);
                    IsentNotice = true;
                }
            }

            return isConnected;
        }


        /// <summary>
        /// Get Current USER Login ID
        /// </summary>
        /// <returns></returns>
        private string GetUserID()
        {
            string username = string.Empty;


            string Sql = "SELECT USER FROM DUAL";

            object reader = ExecuteScalar(Sql);
            username = reader == null ? "" : reader.ToString();

            return username;
        }

        /// <summary>
        /// OracleDataReader
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="paramater"></param>
        /// <returns></returns>
        public OracleDataReader GetDataReader(string SQL, List<OracleParameter> paramater)
        {
            OracleDataReader reader = null;
            reader = GetCommand(CommandType.Text, SQL, paramater).ExecuteReader();
            return reader;
        }

        /// <summary>
        /// Get datareader
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="paramater"></param>
        /// <returns></returns>
        public OracleDataReader GetDataReader(string SQL, params OracleParameter[] paramater)
        {
            OracleDataReader reader = null;
            reader = GetCommand(CommandType.Text, SQL, paramater).ExecuteReader();
            return reader;
        }

        /// <summary>
        /// Get Oracle Command with List of Oracle Param
        /// </summary>
        /// <param name="CmdType"></param>
        /// <param name="SQL"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public OracleCommand GetCommand(CommandType CmdType, String SQL, List<OracleParameter> parameter)
        {
            OracleCommand command = null;
            try
            {
                command = GetCommand();
                command.CommandType = CmdType;
                command.CommandText = SQL;
                if (parameter != null && parameter.Count >= 1)
                {
                    command.BindByName = true;
                    foreach (OracleParameter param in parameter)
                    {
                        command.Parameters.Add(param);
                    }
                }

            }
            catch (OracleException ex)
            {
                ////Lose connection
                if (ORA_ERRORCODE.ORA_CONNECTION_ERROR.Contains(ex.ErrorCode.ToString()))
                {

                    throw new DBManagedDriverConnectionException(ex);
                }
                else
                    throw ex;
            }

            return command;
        }

        /// <summary>
        /// Generate Command with Optional OracleParam
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="SQL"></param>
        /// <param name="parameter">Oracle Parameter is Optional</param>
        /// <returns></returns>
        public OracleCommand GetCommand(CommandType cmdType, String SQL, params OracleParameter[] parameter)
        {
            OracleCommand command = null;
            try
            {
                command = GetCommand();
                command.CommandType = cmdType;
                command.CommandText = SQL;
                if (parameter != null && parameter.Length >= 1)
                {
                    command.BindByName = true;
                    foreach (OracleParameter param in parameter)
                    {
                        command.Parameters.Add(param);
                    }
                }

            }
            catch (OracleException ex)
            {
                if (ORA_ERRORCODE.ORA_CONNECTION_ERROR.Contains(ex.ErrorCode.ToString()))
                {

                    throw new DBManagedDriverConnectionException(ex);

                }
                else
                    throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return command;
        }

        /// <summary>
        /// Execute Scalar
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, List<OracleParameter> param)
        {
            object result = null;

            OracleCommand command = GetCommand(CommandType.Text, sql, param);
            {
                result = command.ExecuteScalar();
            }
            command.Dispose();


            return result;
        }


        /// <summary>
        /// Execute Scalar
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params OracleParameter[] param)
        {
            object result = null;

            OracleCommand command = GetCommand(CommandType.Text, sql, param);
            {
                result = command.ExecuteScalar();
            }
            command.Dispose();

            return result;
        }

        /// <summary>
        /// Get Dataset
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string SQL, List<OracleParameter> param)
        {
            DataSet ds = null;
            OracleCommand command = null;

            command = GetCommand(CommandType.Text, SQL, param);
            {
                ds = new DataSet();
                using (OracleDataAdapter da = new OracleDataAdapter(command))
                    da.Fill(ds);
            }
            command.Dispose();
            return ds;
        }

        /// <summary>
        /// Get Dataset
        /// </summary>
        /// <param name="SQL"></param>
        /// <param name="paramater"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string SQL, params OracleParameter[] paramater)
        {

            //SELECT user FROM dual
            DataSet ds = null;
            OracleCommand command = null;

            command = GetCommand(CommandType.Text, SQL, paramater);

            ds = new DataSet();
            using (OracleDataAdapter da = new OracleDataAdapter(command))
                da.Fill(ds);

            command.Dispose();
            return ds;
        }

        /// <summary>
        /// Execute Not query
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="SQL"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecutNonQuery(CommandType commandType, String SQL, List<OracleParameter> param)
        {
            int Result = 0;

            OracleCommand command = GetCommand(commandType, SQL, param);

            Result = command.ExecuteNonQuery();
            command.Dispose();

            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="SQL"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int ExecutNonQuery(CommandType commandType, String SQL, params OracleParameter[] param)
        {
            int Result = 0;

            OracleCommand command = GetCommand(commandType, SQL, param);

            Result = command.ExecuteNonQuery();

            command.Dispose();
            return Result;
        }

        /// <summary>
        /// Get Command
        /// </summary>
        /// <returns></returns>
        public OracleCommand GetCommand()
        {
            OracleCommand command = new OracleCommand();

            command.Connection = conn;
            //if (IsDBConnected) CheckConnectionTimer.AddMilliseconds(CheckConnection_TimeInterval);
            return command;
        }

        /// <summary>
        /// Get Connection
        /// </summary>
        public OracleConnection GetDBconnection { get { return conn; } }

        /// <summary>
        /// Failover Event. Not Support in Oracle Managed Driver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        //public FailoverReturnCode OnFailover(object sender,
        //                                      OracleFailoverEventArgs eventArgs)
        //{
        //    switch (eventArgs.FailoverEvent)
        //    {
        //        case FailoverEvent.Begin:
        //            DonwnTime = DateTime.Now;
        //            if (!IsentNotice)
        //            {
        //                logger.AddtoLog("Database connection down time : " + DonwnTime.ToString(Functions.dateFormate), LoggerLevel.Fatal);
        //                IsentNotice = true;
        //            }

        //            logger.AddtoLog("Failover Begin - Failing Over ... Please standby", LoggerLevel.Error);
        //            logger.AddtoLog("Failover type was found to be " + eventArgs.FailoverType, LoggerLevel.Error);

        //            break;
        //        case FailoverEvent.Abort:
        //            logger.AddtoLog(" Failover aborted. Failover will not take place.", LoggerLevel.Fatal);
        //            IsentNotice = false;
        //            IsError = false;
        //            break;
        //        case FailoverEvent.End:
        //            UpTime = DateTime.Now;
        //            logger.AddtoLog("Database connection up time : " + UpTime.ToString(Functions.dateFormate), LoggerLevel.Fatal);
        //            logger.AddtoLog(" Failover ended ...resuming services on " + UpTime.ToString(Functions.dateFormate), LoggerLevel.Error);
        //            IsError = false;
        //            IsentNotice = false;
        //            break;

        //        case FailoverEvent.Reauth:
        //            logger.AddtoLog(" Failed over user. Resuming services", LoggerLevel.Error);
        //            break;
        //        case FailoverEvent.Error:


        //            if (!IsError)
        //            {
        //                logger.AddtoLog(" Failover error gotten. Sleeping...", LoggerLevel.Error);
        //                IsError = true;
        //            }

        //            if ((!IsDisposed))
        //            {
        //                Thread.Sleep(500);
        //                return FailoverReturnCode.Retry;
        //            }
        //            else
        //                break;
        //        default:
        //            logger.AddtoLog("Bad Failover Event: " + eventArgs.FailoverEvent, LoggerLevel.Error);
        //            break;
        //    }

        //    return FailoverReturnCode.Success;
        //} /* OnFailover */

        /// <summary>
        /// Create Oracle Parameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataType"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public object CreateParameter(string name, object dataType, object parameterValue)
        {
            return this.CreateParameter(name, dataType, parameterValue, -1, ParameterDirection.Input);
        }

        public object CreateParameter(string name, object dataType, object parameterValue, int size)
        {
            return this.CreateParameter(name, dataType, parameterValue, size, ParameterDirection.Input);
        }

        public object CreatParameterOutput(string name, object dataType, int size)
        {
            return this.CreateParameter(name, dataType, String.Empty, size, ParameterDirection.Output);
        }

        public object CreateParameter(string name, object dataType, object parameterValue, int size, ParameterDirection direction)
        {
            OracleParameter parameter = new OracleParameter(name, (OracleDbType)dataType);

            parameter.Value = parameterValue;
            parameter.Direction = direction;

            if (size >= 0)
            {
                parameter.Size = size;
            }

            return parameter;
        }

        public object CreateReturnParameter()
        {

            OracleParameter returnParameter = new OracleParameter();
            returnParameter.Direction = ParameterDirection.ReturnValue;
            return returnParameter;
        }


        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                logger.AddtoLog("Dispose Database connection", LoggerLevel.Info);
                IsDisposed = true;
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Dispose();
                }
                logger.AddtoLog("Dispose Database connection succeeed", LoggerLevel.Info);

            }
            catch (OracleException ex)
            {
                logger.AddtoLog("Dispose Database connection Failed", LoggerLevel.Error);
                logger.AddtoLog(ex, LoggerLevel.Error);
            }
        }

        #endregion
    }
}

