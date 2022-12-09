using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibLogger;
using System.Collections.Concurrent;
using Asiacell.ITADLibWCF.Business;
using Asiacell.ITADLibraries.LibDatabase;
using Asiacell.ITADLibWCF.Service;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace Asiacell.ITADLibWCF.Classes
{
    /// <summary>
    /// Singleton Server Object class
    /// </summary>
    public sealed class ServerObject : ServerObjectAbstract
    {
        // Singleton instance
        private static volatile ServerObject _instance;
        // safe thread for initialize instance of the class
        private static object syncRoot = new Object();
        private bool _isStopScheduler { get; set; }
        private Task _scheduler;
        static private int Interval=100;

        private LoggerEntities logger = null;
        
        # region Properties
        public ConcurrentDictionary<string, User> Users
        {
            get
            {
                return this._users;
            }
        }
        public ConcurrentDictionary<string, ClientIP> ClientIps
        {
            get { return this._clientIps; }
        }
        public ConcurrentDictionary<string, SystemCommand> SystemCommands
        {
            get { return this._systemCommands; }
        }

         public ConcurrentDictionary<int, Endpoint> Endpoints
        {
            get { return this._endpoints; }
        }
        # endregion

        /// <summary>
        /// Constructor of Servers object
        /// </summary>
        /// <param name="users"></param>
        /// <param name="clientIps"></param>
        private ServerObject(DBConnection db,int Interval ,LoggerEntities logger)
            : base(logger,db)
        {
            // Code that runs on application startup
            //log4net.Config.XmlConfigurator.Configure();              
            this.logger = logger;
            ServerObject.Interval = Interval;
            lock (this)
            {
                this.SyncWithDB();
            }
            // Start Thread Here
            this._isStopScheduler = false;

            //_scheduler = new Task(this.SyncScheduler);
            //_scheduler.Start();
        }


        /// <summary>
        /// Get Server object instance
        /// </summary>
        public static ServerObject Instance(LoggerEntities logger, int interval, DBConnection db)
        {
            if (_instance == null)
            {

                lock (syncRoot) // thread safe
                {
                    logger.AddtoLog("Check singleton server object", LoggerLevel.Info);
                    if (_instance == null)
                        _instance = new ServerObject(db, interval, logger);
                }
            }

            return _instance;
        }

        /// <summary>
        /// Sync database scheduler
        /// </summary>
        private void SyncScheduler()
        {

            while (!(this._isStopScheduler))
            {
                // exit the thread block                
                // execute the sync database process
                lock (this)
                {
                    this.SyncWithDB();
                }
                Thread.Sleep(Interval * 1000);
            }
            // end of the scheduling block
        }

        public void StopScheduler()
        {
            this._isStopScheduler = true;
        }

        public bool IsValidUser(string username, ref RequestObject requestObject, out User pUser)
        {
            // if the user is already existed
            if (this._users.ContainsKey(username))
            {
                User user = this._users[username];

                // filter out property
                if (user.userName == username)
                {
                    logger.AddtoLog(requestObject.Id, "Authentication : User is authorized", LoggerLevel.Info);
                    requestObject.Description = "Authenticate : User is authorized";
                    pUser = user;
                    return true;
                }
                else
                {
                    logger.AddtoLog(requestObject.Id, "Error authentication : User is unauthorized", LoggerLevel.Error);
                    requestObject.Description = "Authenticate : User is unauthorized";
                    requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                    pUser = null;
                    return false;
                }
            }
            else
            {
                logger.AddtoLog(requestObject.Id, "Authenticate : Non-exists user.", LoggerLevel.Error);
                requestObject.Description = "Authenticate : Non-exists user.";
                requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                pUser = null;
                return false;
            }
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string username, string password, ref RequestObject requestObject, out User pUser)
        {
            pUser = new User();
            // check client ip address
            var ip = Asiacell.ITADLibWCF.Utilities.WCFUtilities.GetRequestedClientIP(); //ITADServerObject.Utilities.WCFUtilities.GetRequestedClientIP();

            //ip = "127.0.0.1";           // for test 
            if (ip == "")
            {
                //logger.AddtoLog(requestObject.Id, "Authenticate : Cannot get client ip address", LoggerLevel.Error);
                requestObject.Description = "Authenticate : Cannot get client ip address";         
                requestObject.Error_Code = SystemErrorCodes.Client_UnauthaurizeIP;
                return false;
            }
            
            // if client ip address is allowed
            if (this._clientIps.ContainsKey(ip))
            {
                // check username and password in the memory collection
                if (username == null)
                {
                    //logger.AddtoLog(requestObject.Id, "Authenticate : Username is null", LoggerLevel.Error);
                    requestObject.Description = "Authenticate : Username is null";         
                    requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                    return false;
                }
                if (password == null)
                {
                    //logger.AddtoLog(requestObject.Id, "Authenticate : Passowrd is null", LoggerLevel.Error);
                    requestObject.Description = "Authenticate : Passowrd is null";         
                    requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                    return false;
                }                
                
                // if the user is already existed
                if (this._users.ContainsKey(username))
                {
                    User user = this._users[username];

                    // filter out property
                    if (user.userName.ToLower() == username.ToLower() && user.password == password)
                    {
                        //logger.AddtoLog(requestObject.Id, "Authentication : User is authorized", LoggerLevel.Info);
                        requestObject.Description = "Authenticate : User is authorized";
                        pUser = user;
                        return true;
                    }
                    else
                    {
                        //logger.AddtoLog(requestObject.Id, "Error authentication : User is unauthorized", LoggerLevel.Error);
                        requestObject.Description = "Authenticate : User is unauthorized";         
                        requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                        return false;
                    }
                }
                else
                {
                    //logger.AddtoLog(requestObject.Id, "Authenticate : Non-exists user.", LoggerLevel.Error);
                    requestObject.Description = "Authenticate : Non-exists user.";         
                    requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                    return false;
                }
            }
            else
            {
               // logger.AddtoLog(requestObject.Id,"Client IP address is unauthorized", LoggerLevel.Error);
                requestObject.Description = "Client IP address is unauthorized";         
                requestObject.Error_Code = SystemErrorCodes.Client_UnauthaurizeIP;
                //result = false; // client ip is not allow
                return false;
            }
        }

        /// <summary>
        /// Validate Command
        /// </summary>
        /// <param name="systemCommandRequested"></param>
        /// <param name="nameValuePair"></param>
        /// <returns></returns>
        public bool ValidateCommand(string systemCommandRequested, User user, ref RequestObject requestObject, ref CommandOutputFilter patternFilter, out FilterSensitive filterSensitive)
        {
            // validate command
            if (requestObject.Equals(null))
                requestObject = new RequestObject();

            string[] commandParts = Functions.SpliteBySplace(systemCommandRequested);
            string commandName = commandParts[0].ToUpper().Trim();             

            filterSensitive = this._filterSensitive.Values.FirstOrDefault(s =>
               (s.CommandName == commandName));
 
          
            int msisdnIndex = 1; // default first params is isdn it canbe change base on element's system command.

            // start to validate the command to element server
            List<NameValuePair> nameValuePair = new List<NameValuePair>();

            // check system command name in the database
            if (!(this.SystemCommands.ContainsKey(commandName)))
            {
                requestObject.Error_Code = Asiacell.ITADLibraries.Utilities.SystemErrorCodes.Client_Unknow_Command;
                requestObject.Description = "ValidateCommand : Unknow system command.";         
                logger.AddtoLog(requestObject.Id,"ValidateCommand : Unknow system command.", LoggerLevel.Error);
                return false;
            }

            // check permission
            var systemCommand = this.SystemCommands[commandName]; 
            //this.SystemCommands.Values.Where(s => s.CommandName.ToUpper() == commandName.ToUpper()).ElementAt(0);

            // check if the command permission is defined
            var commandPermission = user.commandPermission.Trim();

            // if full permission or has permission to the command
            if(commandPermission.Contains("[*]")
            || commandPermission.Contains(String.Format("[{0}]", commandName))
            || commandPermission.Contains(String.Format("[{0}]", systemCommand.SystemCommandId)))
            {
                // do nothing and mean client has permission to execute command
            }
            else
            {
                requestObject.Error_Code = Asiacell.ITADLibraries.Utilities.SystemErrorCodes.Client_Has_No_Command_Permission;
                requestObject.Description = String.Format("Client has no permission to execute the command");
                logger.AddtoLog(requestObject.Id, String.Format("User:{0} has no permission to execute the command:{1}", requestObject.UserName, commandName), LoggerLevel.Error);
                return false;
            }

            // add command name
            requestObject.Command_Name = commandName;
            requestObject.CommandID = systemCommand.SystemCommandId;

            var systemCommandId = systemCommand.SystemCommandId; 

            // command is exists check params validation
            string[] commandParams = systemCommand.CommandParam.Trim().Split(',');
            
            // pair number of parameters
            if (!(commandParams.Length == commandParts.Length - 1))
            {
                requestObject.Error_Code = SystemErrorCodes.Client_Invalid_Parameter;
                requestObject.Description = "ValidateCommand : Mismatch parameters.";
                logger.AddtoLog(requestObject.Id, "ValidateCommand : Mismatch parameters.", LoggerLevel.Error);
                return false;
            }

            //if (commandParams.Length == 1 && commandParts.Length == 1)
            //{
            //    // command with no parameter
            //}
            
            // get command output filter pattern by user
            var filter = this._commandOutputFilters.Values.FirstOrDefault(s =>
                (s.UserId == user.userId && s.SystemCommandId == systemCommandId && s.ApplyOn == "U")); 

            // get pattern by group
            if (filter == null)
                filter = this._commandOutputFilters.Values.FirstOrDefault(s =>
                    (s.GroupId == user.userId && s.SystemCommandId == systemCommandId && s.ApplyOn == "G"));

            if(filter != null){
                patternFilter.ApplyOn = filter.ApplyOn;
                patternFilter.CommandFilterId = filter.CommandFilterId;
                patternFilter.Pattern = filter.Pattern;
                patternFilter.UserId = filter.UserId;
                patternFilter.GroupId = filter.GroupId;
            }

            // check MSSIDN or IMSI and gather into key pair parameter values
            if (commandParts.Length > 1)
            {
                bool checkForMsisdnOrImsi = false;

                //// check through to find the msdn parameters to move to the first param of the params list.
                if (systemCommand.IsImsi == 1 || systemCommand.IsIsdn == 1)
                {
                    checkForMsisdnOrImsi = true;
                }
                
                for (int i = 0; i < commandParams.Length; i++)
                {
                    string p = commandParams[i];

                    // check valid MSISDN or IMSI
                    if ((p.ToLower().Equals("%isdn%") || 
                        p.ToLower().Equals("%msisdn%") ||
                        p.ToLower().Equals("%imsi%")) &&
                        checkForMsisdnOrImsi &&
                        String.IsNullOrEmpty(p) == false &&
                        p.ToLower() != "null"
                        )
                    {
                        msisdnIndex = i + 1;

                        string msisdnOrImsi = commandParts[msisdnIndex].Trim();

                        // no longer use due to some command can be contained no argument
                        // check the first parameter value must be MSISDN or IMSI
                        if (Functions.IsMSISDN(msisdnOrImsi) || Functions.IsIMSI(msisdnOrImsi))
                        {
                            // Keep continues
                        }
                        else
                        {
                            // respond any error
                            requestObject.Error_Code = SystemErrorCodes.Client_Invalid_MSISDNorIMSI;
                            requestObject.Description = "ValidateCommand : Invalid MSIDN or IMSI parameter.";
                            logger.AddtoLog(requestObject.Id, "ValidateCommand : Invalid MSIDN or IMSI parameter.", LoggerLevel.Error);
                            return false;
                        }
                    }
                }
                
                // msisdn parameter at the first of params collection
                nameValuePair.Add(new NameValuePair { Name = commandParams[msisdnIndex - 1], Value = commandParts[msisdnIndex].Trim() });
               
                // gather into key pairs collection
                for (int i = 1; i < commandParts.Length; i++)
                {
                    if (msisdnIndex != i)
                    {
                        nameValuePair.Add(new NameValuePair { Name = commandParams[i - 1], Value = commandParts[i].Trim().Replace("\"","") });
                    }
                }
            }

            try
            {
                // apply command info to request object
                requestObject.Command_Name = systemCommand.CommandName;
                requestObject.CommandID = systemCommand.SystemCommandId;
                requestObject.Element_Id = systemCommand.ElementTypeId;
                requestObject.CommandParamaterValues = nameValuePair;
            }
            catch (Exception e)
            {
                requestObject.Description = "ValidateCommand : Invalid MSIDN or IMSI parameter.";
                requestObject.Error_Code = SystemErrorCodes.Client_Invalid_Parameter;
                logger.AddtoLog(requestObject.Id , "ValidateCommand : Failed to set the request object properties", LoggerLevel.Error);
            }
            return true;
        }

        /// <summary>
        /// Apply filtering output as JObject
        /// </summary>
        /// <param name="patternFilter"></param>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public JObject ApplyFilterJObjectOutput(CommandOutputFilter patternFilter, string jsonString)
        {
            JsonObject jsonObject = new JsonObject();
            if (String.IsNullOrWhiteSpace(jsonString) == false)
            {
                return jsonObject.ApplyFilter(patternFilter.Pattern, jsonString);
            }
            else
            {
                return new JObject();
            }
        }

        /// <summary>
        /// Apply filter out put as JsonString
        /// </summary>
        /// <param name="patternFilter"></param>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public string ApplyFilterJsonStringOutput(CommandOutputFilter patternFilter, string jsonString)
        {
            JsonObject jsonObject = new JsonObject();
            if (String.IsNullOrWhiteSpace(jsonString) == false)
            {
                return JsonConvert.SerializeObject(jsonObject.ApplyFilter(patternFilter.Pattern, jsonString));
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// Authenticate with command
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="systemCommand"></param>
        /// <returns></returns>
        public bool Authenticate(string username, string password, string systemCommand, ref RequestObject requestObject, ref CommandOutputFilter patternFilter, out FilterSensitive filterSensitive)
        {
            if (requestObject.Equals(null))
                requestObject = new RequestObject();

            User user;
            filterSensitive = null;
            

            // if you authentication is success
            bool result = this.Authenticate(username, password, ref requestObject, out user);
             

            // validate command
            if (result == true)
            {
                // validate and check for authorization to execute the command
                // whether the user has permission to call the command or not!
                result = this.ValidateCommand(systemCommand, user, ref requestObject, ref patternFilter, out filterSensitive);

                if (result == false)
                {
                    return result;
                }
            }
            else
            {
                return result;
            }

            return result;
        }
        public bool Authenticate(string username, string password, string systemCommand, ref RequestObject requestObject, 
            ref CommandOutputFilter patternFilter, out FilterSensitive filterSensitive, out User user)
        {
            if (requestObject.Equals(null))
                requestObject = new RequestObject();

            filterSensitive = null;
            // if you authentication is success
            bool result = this.Authenticate(username, password, ref requestObject, out user);


            // validate command
            if (result == true)
            {
                // validate and check for authorization to execute the command
                // whether the user has permission to call the command or not!
                result = this.ValidateCommand(systemCommand, user, ref requestObject, ref patternFilter, out filterSensitive);

                if (result == false)
                {
                    return result;
                }
            }
            else
            {
                return result;
            }

            return result;
        }


    }
}
