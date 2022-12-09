using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using Oracle.DataAccess.Client;
using Asiacell.ITADLibraries_v1.LibDatabase;
using Asiacell.ITADLibraries_v1.Utilities;
using Asiacell.ITADLibraries_v1.LibLogger;
using Asiacell.ITADLibWCF_v1.Business;

namespace Asiacell.ITADLibWCF_v1.Classes
{
    public abstract class ServerObjectAbstract
    {
        // Memory store of User and ClientIP
        protected ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();
        protected ConcurrentDictionary<string, ClientIP> _clientIps = new ConcurrentDictionary<string, ClientIP>();
        protected ConcurrentDictionary<string, SystemCommand> _systemCommands = new ConcurrentDictionary<string, SystemCommand>();

        private LoggerEntities logger = null;
        private DBConnection db = null;

        public ServerObjectAbstract(LoggerEntities logger, DBConnection db)
        {
            this.logger = logger;
            this.db = db;

        }

        /// <summary>
        /// Sync users collection
        /// </summary>
        public void SyncWithDB()
        {
            //log4net.ILog logger = log4net.LogManager.GetLogger(GetType());

            //List<int> updatedUserId = new List<int>();

            try
            {

                // produce connection to Oracle Database
                //using (DBConnection dbApps = new DBConnection(DbConnectionString, logger))
                //{
                //if (dbApps.StartupConnection())                
                //{
                lock (this)
                {
                    // Get users collections
                    GetUsers();

                    // Get IP client collections
                    GetClientIP();

                    // Get System command collections
                    GetSystemCommands();
                }
                //}
                //else
                //{
                //    logger.AddtoLog("Unable to connect to database, Please check in log file for detail", LoggerLevel.Error);
                //}
                //}
            }
            catch (Exception ex)
            {
                logger.AddtoLog(ex, LoggerLevel.Error);
            }
        }


        /// <summary>
        /// Get users data and add into users collection
        /// </summary>
        /// <param name="users"></param>
        /// <param name="dbApps"></param>
        private void GetUsers()
        {
            string sqlUser = "select userid, user_name, password, user_type, status, groupid, command_permission, max_pool_size, connection_timeout from tbl_user where status =:status";

            OracleParameter para = new OracleParameter("status", OracleDbType.Int16);
            para.Value = 1;

            ICollection<string> toBeRemoved = _users.Keys;

            // collect user information to collection users
            //using (OracleDataReader reader = db.GetDataReader(sqlUser, para))
            using(DataSet ds = db.GetDataSet(sqlUser, para))
            {
                //while (reader.Read())
                foreach(DataRow reader in ds.Tables[0].Rows)
                {
                    // read user data from database
                    User user = new User
                    {
                        userId = System.Convert.ToInt32(reader["userid"]),
                        userName = System.Convert.ToString(reader["user_name"]),
                        password = System.Convert.ToString(reader["password"]),
                        userType = System.Convert.ToInt32(reader["user_type"]),
                        status = System.Convert.ToInt32(reader["status"]),
                        groupId = System.Convert.ToInt32(reader["groupid"]),
                        commandPermission = System.Convert.ToString(reader["command_permission"]),
                        maxPoolSize = System.Convert.ToInt32(reader["max_pool_size"]),
                        connectionTimeout = System.Convert.ToInt32(reader["connection_timeout"])
                    };

                    //check in the collection of users data.
                    _users.AddOrUpdate(user.userName, user, (key, oldUser) => user);

                    // add to remove list
                    try { toBeRemoved.Remove(user.userName); }
                    catch (Exception) { }
                }

                // remove non exists users from the memory
                try
                {
                    IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                    foreach (string userName in removeItemList)
                    {
                        User userReturn;
                        _users.TryRemove(userName, out userReturn);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Get client ip address and add to collection clientIps
        /// </summary>
        /// <param name="clientIps"></param>
        /// <param name="dbApps"></param>
        private void GetClientIP()
        {
            string sqlClientIP = "select clientip, client_name, created_on, created_by, updated_on, updated_by, remark from tbl_client_ip";

            ICollection<string> toBeRemoved = _clientIps.Keys;
            OracleParameter param = new OracleParameter();
            // get client ip address
            //using (OracleDataReader reader = db.GetDataReader(sqlClientIP))
            using(DataSet ds = db.GetDataSet(sqlClientIP))
            {
                //while (reader.Read())
                foreach(DataRow reader in ds.Tables[0].Rows)
                {
                    // read user data from database
                    ClientIP clientIp = new ClientIP
                    {
                        clientip = Convert.ToString(reader["clientip"]),
                        clientName = Convert.ToString(reader["client_name"])
                    };

                    //check in the collection of users data.
                    _clientIps.AddOrUpdate(clientIp.clientip, clientIp, (key, oldClientIp) => clientIp);
                    try { toBeRemoved.Remove(clientIp.clientip); }
                    catch (Exception e) { }
                }
                try
                {
                    // remove non exists client ip from the memory
                    IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                    foreach (string ip in removeItemList)
                    {
                        ClientIP clientIpReturn;
                        _clientIps.TryRemove(ip, out clientIpReturn);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// GetSystemCommands
        /// </summary>
        /// <param name="systemCommands"></param>
        /// <param name="dbApps"></param>
        private void GetSystemCommands()
        {
            string sql = "select systemcommandid, command_name, elementtypeid, command_param, internalparam, description, created_on, created_by, updated_on, updated_by, remark from tbl_system_command";

            ICollection<string> toBeRemoved = _systemCommands.Keys;

            // get client ip address
            //using (OracleDataReader reader = db.GetDataReader(sql))
            
            using(DataSet ds = db.GetDataSet(sql))
            {
                //while (reader.Read())
                foreach(DataRow reader in ds.Tables[0].Rows)
                {
                    // read user data from database
                    SystemCommand systemCommand = new SystemCommand
                    {
                        SystemCommandId = Convert.ToInt32(reader["systemcommandid"]),
                        CommandName = Convert.ToString(reader["command_name"]),
                        ElementTypeId = Convert.ToInt32(reader["elementtypeid"]),
                        CommandParam = Convert.ToString(reader["command_param"]),
                        InternalParam = Convert.ToString(reader["internalparam"])
                    };

                    //check insystemCommands the collection of users data.
                    _systemCommands.AddOrUpdate(systemCommand.CommandName, systemCommand, (key, old) => systemCommand);
                    try { toBeRemoved.Remove(systemCommand.CommandName); }
                    catch (Exception e) { }
                }
                try
                {
                    // remove non exists client ip from the memory
                    IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                    foreach (string command in removeItemList)
                    {
                        SystemCommand systemCommandReturn;
                        _systemCommands.TryRemove(command, out systemCommandReturn);
                    }
                }
                catch { }
            }
        }


    }
}
