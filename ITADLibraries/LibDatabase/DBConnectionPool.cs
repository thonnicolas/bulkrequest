using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Asiacell.ITADLibraries.LibLogger;
using System.Threading.Tasks;

namespace Asiacell.ITADLibraries.LibDatabase
{
    public class DBConnectionPool
    {
        private readonly int numofConnection = 0;
        private static int increment = 0;
        private DBConnection[] dbPool = null;

        private LoggerEntities logger;
        private string ConnectionString = string.Empty;
        private string Keys = "";
        private Boolean Encryption = false;
        /// <summary>
        /// Contractor without encryption
        /// </summary>
        /// <param name="NumberOfPool">Number of open connection pool</param>
        /// <param name="ConnectionString">Constring string</param>
        /// <param name="logger">Logger Object</param>
        public DBConnectionPool(int NumberOfPool, string ConnectionString, LoggerEntities logger)
        {
            numofConnection = NumberOfPool;
            dbPool = new DBConnection[numofConnection];
            this.logger = logger;
            this.ConnectionString = ConnectionString;
        }

        /// <summary>
        /// Contractor wtih encryption
        /// </summary>
        /// <param name="NumberOfPool">Number of open connection pool</param>
        /// <param name="ConnectionString">Constring string</param>
        /// <param name="logger">Logger Object</param>
        public DBConnectionPool(int NumberOfPool, string ConnectionString, String key, LoggerEntities logger)
        {
            Keys = key;
            Encryption = true;
            numofConnection = NumberOfPool;
            dbPool = new DBConnection[numofConnection];
            this.logger = logger;
            this.ConnectionString = ConnectionString;
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
        public DBConnection GetConnection()
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
                    if (!Encryption)
                    {
                        dbPool[i] = new DBConnection(ConnectionString, logger);
                    }
                    else
                    {
                        dbPool[i] = new DBConnection(ConnectionString, Keys, logger);
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
