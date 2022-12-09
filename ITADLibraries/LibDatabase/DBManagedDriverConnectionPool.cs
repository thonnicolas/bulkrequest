using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asiacell.ITADLibraries.LibLogger;
using System.Threading;
using System.Threading.Tasks;
using Asiacell.ITADLibraries.Utilities;

namespace Asiacell.ITADLibraries.LibDatabase
{

    public class DBManagedDriverConnectionPool
    {
        private readonly int numofConnection = 0;
        private static int increment = 0;
        private DBManagedDriverConnection[] dbPool = null;

        private LoggerEntities logger;
        private string ConnectionString = string.Empty;
        private bool encrption = false;
        private string Keys = "";

        /// <summary>
        /// Contructor. Using Plain Text 
        /// </summary>
        /// <param name="NumberOfPool">Number of open connection pool</param>
        /// <param name="ConnectionString">Constring string</param>
        /// <param name="Encryption">Encrption</param>
        /// <param name="Keys">Key encrption</param>
        /// <param name="logger">Logger Object</param>
        public DBManagedDriverConnectionPool(int NumberOfPool, string ConnectionString, LoggerEntities logger)
        {
            numofConnection = NumberOfPool;
            dbPool = new DBManagedDriverConnection[numofConnection];
            this.logger = logger;
            this.encrption = false;
            this.ConnectionString = ConnectionString;
            this.Keys = "";
        }

        /// <summary>
        /// Contructor. Using Encryption Key
        /// </summary>
        /// <param name="NumberOfPool">Number of open connection pool</param>
        /// <param name="ConnectionString">Constring string</param>
        /// <param name="Encryption">Encrption</param>
        /// <param name="Keys">Key encrption</param>
        /// <param name="logger">Logger Object</param>
        public DBManagedDriverConnectionPool(int NumberOfPool, string ConnectionString, String Keys, LoggerEntities logger)
        {
            numofConnection = NumberOfPool;
            dbPool = new DBManagedDriverConnection[numofConnection];
            this.logger = logger;
            this.encrption = true;
            this.ConnectionString = ConnectionString;
            this.Keys = Keys;
        }

        /// <summary>
        /// Get Pool array index
        /// </summary>
        /// <returns></returns>
        private int GetIndex()
        {

            lock (this)
            {
                if (increment >= numofConnection - 1)
                    increment = 0;
                else
                    Interlocked.Increment(ref increment);
                return increment;
            }

        }

        /// <summary>
        /// Get Database Connection
        /// </summary>
        /// <returns></returns>
        public DBManagedDriverConnection GetConnection()
        {
            int ind = 0;
            ind = GetIndex();
            return dbPool[ind];

        }


        /// <summary>
        /// Initialize Connection Pool
        /// </summary>
        public void StartUpDBPool()
        {
            try
            {
                Task[] dbTask = new Task[numofConnection];

                for (int i = 0; i < numofConnection; i++)
                {
                    if (!encrption)
                    {
                        dbPool[i] = new DBManagedDriverConnection(ConnectionString,  logger);
                    }
                    else
                    {
                        dbPool[i] = new DBManagedDriverConnection(ConnectionString,Keys, logger);
                    }

                    //Action aa = new Action(dbPool[i].StartupConnection());

                    dbTask[i] = Task.Factory.StartNew((ind) => { dbPool[(int)ind].StartupConnection(); }, i);

                }

                Task.WaitAll(dbTask);
            }
            catch (Exception ex)
            {
                logger.AddtoLog(ex, LoggerLevel.Error);
            }
        }

        /// <summary>
        /// Dispose all Database connection
        /// </summary>
        public void Disposed()
        {

            for (int i = 0; i < numofConnection; i++)
            {
                try
                {
                    if (dbPool[i].GetDBconnection.State == System.Data.ConnectionState.Open)
                        dbPool[i].Dispose();
                }
                catch (Exception ex)
                {
                    logger.AddtoLog(ex, LoggerLevel.Error);
                }
            }
        }

    }
}
