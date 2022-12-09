using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;
using Asiacell.ITADLibraries.LibDatabase;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibWCF.Business;

namespace Asiacell.ITADLibWCF.Classes
{
    public abstract class ServerObjectAbstract
    {
        // Memory store of User and ClientIP
        protected ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();
        protected ConcurrentDictionary<string, ClientIP> _clientIps = new ConcurrentDictionary<string, ClientIP>();
        protected ConcurrentDictionary<string, SystemCommand> _systemCommands = new ConcurrentDictionary<string, SystemCommand>();
        protected ConcurrentDictionary<int, CommandOutputFilter> _commandOutputFilters = new ConcurrentDictionary<int, CommandOutputFilter>();
        protected ConcurrentDictionary<string, FilterSensitive> _filterSensitive = new ConcurrentDictionary<string, FilterSensitive>();
        protected ConcurrentDictionary<int, Endpoint> _endpoints = new ConcurrentDictionary<int, Endpoint>();

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
            try
            {
                lock (this)
                {
                    // Get users collections
                    GetUsers();

                    // Get IP client collections
                    GetClientIP();

                    // Get System command collections
                    GetSystemCommands();

                    // Get User command filtering pattern
                    GetCommandOutputFilters();

                    // Sensitive Filter
                    GetSensitiveFilters();

                    // Get EndPoints
                    GetEndpoints();
                }
               
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
            string sqlUser = "select userid, user_name, password, user_type, status, groupid, command_permission, max_pool_size, connection_timeout, queue_multiply_by from tbl_user where status =:status";

            OracleParameter para = new OracleParameter("status", OracleDbType.Int16);
            para.Value = 1;
            
            string[] removed = new string[_users.Keys.Count];
            _users.Keys.CopyTo(removed, 0);
            List<string> toBeRemoved = removed.ToList<string>();


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
                        connectionTimeout = System.Convert.ToInt32(reader["connection_timeout"]),
                        queueMultiplyBy = System.Convert.ToInt32((reader["queue_multiply_by"] == DBNull.Value) ? "10" : reader["queue_multiply_by"])
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
                    //IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                    foreach (string userName in toBeRemoved)
                    {
                        User userReturn;
                        _users.TryRemove(userName, out userReturn);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Get Sensitive Filter Configuration
        /// </summary>
        /// <param name="users"></param>
        /// <param name="dbApps"></param>
        private void GetSensitiveFilters()
        {
            string sqlSelect = @"select sensitive_params, command_name from tbl_filter_sensitive";


            string[] removed = new string[_filterSensitive.Keys.Count];
            _filterSensitive.Keys.CopyTo(removed, 0);
            List<string> toBeRemoved = removed.ToList<string>();


            // collect user command information
            //using (OracleDataReader reader = db.GetDataReader(sqlUser, para))
            using (DataSet ds = db.GetDataSet(sqlSelect))
            {
                //while (reader.Read())
                foreach (DataRow reader in ds.Tables[0].Rows)
                {
                    // read user command data from database
                    FilterSensitive commandOutputFilter = new FilterSensitive
                    {
                        CommandName = Functions.ToString(reader["command_name"]),
                        SensitiveParams = Functions.ToString(reader["sensitive_params"])
                    };

                    //check in the collection of user command data.
                    _filterSensitive.AddOrUpdate(commandOutputFilter.CommandName, commandOutputFilter, (key, old) => commandOutputFilter);

                    // add to remove list
                    try { toBeRemoved.Remove(commandOutputFilter.CommandName); }
                    catch (Exception) { }
                }

                // remove non exists users from the memory
                try
                {
                    //IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                    foreach (string commandFilterName in toBeRemoved)
                    {
                        FilterSensitive userCommandReturn;
                        _filterSensitive.TryRemove(commandFilterName, out userCommandReturn);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Get users data and add into users collection
        /// </summary>
        /// <param name="users"></param>
        /// <param name="dbApps"></param>
        private void GetCommandOutputFilters()
        {
            string sqlUserCommand = @"select commandfilterid, userid, systemcommandid, pattern, 
                                        groupid, apply_on 
                                    from TBL_COMMAND_OUTPUT_FILTER";

      
            int[] removed = new int[_commandOutputFilters.Keys.Count];
            _commandOutputFilters.Keys.CopyTo(removed, 0);
            List<int> toBeRemoved = removed.ToList<int>();


            // collect user command information
            //using (OracleDataReader reader = db.GetDataReader(sqlUser, para))
            using (DataSet ds = db.GetDataSet(sqlUserCommand))
            {
                //while (reader.Read())
                foreach (DataRow reader in ds.Tables[0].Rows)
                {
                    // read user command data from database
                    CommandOutputFilter commandOutputFilter = new CommandOutputFilter
                    {
                        CommandFilterId = Functions.ToNumber(reader["commandfilterid"]),
                        Pattern = Functions.ToString(reader["pattern"]),
                        UserId = Functions.ToNumber(reader["userid"]),
                        SystemCommandId = Functions.ToNumber(reader["systemcommandid"]),
                        GroupId = Functions.ToNumber(reader["groupid"]),
                        ApplyOn = Functions.ToString(reader["apply_on"])
                    };

                    //check in the collection of user command data.
                    _commandOutputFilters.AddOrUpdate(commandOutputFilter.CommandFilterId, commandOutputFilter, (key, oldUser) => commandOutputFilter);

                    // add to remove list
                    try { toBeRemoved.Remove(commandOutputFilter.CommandFilterId); }
                    catch (Exception) { }
                }

                // remove non exists users from the memory
                try
                {
                    //IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                    foreach (int commandFilterId in toBeRemoved)
                    {
                        CommandOutputFilter userCommandReturn;
                        _commandOutputFilters.TryRemove(commandFilterId, out userCommandReturn);
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

            //ICollection<string> toBeRemoved = _clientIps.Keys;

            string[] removed = new string[_clientIps.Keys.Count];            
            _clientIps.Keys.CopyTo(removed, 0);
            List<string> toBeRemoved = removed.ToList<string>();

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
                    //IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();

                    foreach (string ip in toBeRemoved)
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
            string sql = @"select systemcommandid, command_name, elementtypeid, command_param, internalparam, 
                            description, created_on, created_by, updated_on, updated_by, 
                            is_isdn, is_imsi, remark 
                            from tbl_system_command";

            //ICollection<string> toBeRemoved = _systemCommands.Keys;            

            string[] removed = new string[_systemCommands.Keys.Count];
            _systemCommands.Keys.CopyTo(removed, 0);
            List<string> toBeRemoved = removed.ToList<string>();

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
                        InternalParam = Convert.ToString(reader["internalparam"]),
                        IsIsdn = Functions.ToNumber(reader["is_isdn"]),
                        IsImsi = Functions.ToNumber(reader["is_imsi"])
                    };

                    //check insystemCommands the collection of users data.
                    _systemCommands.AddOrUpdate(systemCommand.CommandName, systemCommand, (key, old) => systemCommand);
                    
                    try { 
                        toBeRemoved.Remove(systemCommand.CommandName);                         
                    }
                    catch (Exception e) { }
                }
                try
                {
                    // remove non exists client ip from the memory
                    //IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                    foreach (string command in toBeRemoved)
                    {
                        SystemCommand systemCommandReturn;
                        _systemCommands.TryRemove(command, out systemCommandReturn);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Get Endpoints
        /// </summary>
        /// <param name="systemCommands"></param>
        /// <param name="dbApps"></param>
        private void GetEndpoints()
        {
            string sql = @"select endpointid, endpoint_url, status, type, elementtypeid 
                           from tbl_endpoint 
                            where status = 1";
             
            int[] removed = new int[_systemCommands.Keys.Count];
            _endpoints.Keys.CopyTo(removed, 0);
            List<int> toBeRemoved = removed.ToList<int>();
            
            using(DataSet ds = db.GetDataSet(sql))
            {
                //while (reader.Read())
                foreach(DataRow reader in ds.Tables[0].Rows)
                {
                    // read user data from database
                    Endpoint endpoint = new Endpoint
                    {
                        EndpointId = Convert.ToInt32(reader["endpointid"]),
                        EndpointUrl = Convert.ToString(reader["endpoint_url"]),
                        Status = Convert.ToInt32(reader["status"]),
                        Type = Convert.ToString(reader["type"]),
                        ElementTypeId = Functions.ToNumber(reader["elementtypeid"])
                    };

                    //check insystemCommands the collection of users data.
                    _endpoints.AddOrUpdate(endpoint.EndpointId, endpoint, (key, old) => endpoint);
                    
                    try { 
                        toBeRemoved.Remove(endpoint.EndpointId);                         
                    }
                    catch (Exception e) { }
                }
                try
                {
                    // remove non exists client ip from the memory
                    //IEnumerable<string> removeItemList = (IEnumerable<string>)toBeRemoved.GetEnumerator();
                    foreach (int command in toBeRemoved)
                    {
                        Endpoint removeOut;
                        _endpoints.TryRemove(command, out removeOut);
                    }
                }
                catch { }
            }
        }
    }
}
