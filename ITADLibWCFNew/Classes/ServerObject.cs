using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asiacell.ITADLibraries_v1.Utilities;
using Asiacell.ITADLibraries_v1.LibLogger;
using System.Collections.Concurrent;
using Asiacell.ITADLibWCF_v1.Business;
using Asiacell.ITADLibraries_v1.LibDatabase;
using Asiacell.ITADLibWCF_v1.Service;


namespace Asiacell.ITADLibWCF_v1.Classes
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
        
        
        //log4net.ILog _logger;
        //log4net.ILog logger = log4net.LogManager.GetLogger("ServerObject");

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
            //get
            //{

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
            //}
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


        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string username, string password, ref RequestObject requestObject)
        {
            
            // check client ip address
            var ip = Asiacell.ITADLibWCF_v1.Utilities.WCFUtilities.GetRequestedClientIP(); //ITADServerObject.Utilities.WCFUtilities.GetRequestedClientIP();
            ip = "127.0.0.1";           // for test 
            if (ip == "")
            {
                logger.AddtoLog(requestObject.Id, "Authenticate : Cannot get client ip address", LoggerLevel.Error);
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
                    logger.AddtoLog(requestObject.Id, "Authenticate : Username is null", LoggerLevel.Error);
                    requestObject.Description = "Authenticate : Username is null";         
                    requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                    return false;
                }
                if (password == null)
                {
                    logger.AddtoLog(requestObject.Id, "Authenticate : Passowrd is null", LoggerLevel.Error);
                    requestObject.Description = "Authenticate : Passowrd is null";         
                    requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                    return false;
                }                
                
                if (this._users.ContainsKey(username))
                {
                    User user = this._users[username];
                    if (user.userName == username && user.password == password)
                    {
                        logger.AddtoLog(requestObject.Id, "Authentication : User is authorized", LoggerLevel.Info);
                        requestObject.Description = "Authenticate : User is authorized";         
                        return true;
                    }
                    else
                    {
                        logger.AddtoLog(requestObject.Id, "Error authentication : User is unauthorized", LoggerLevel.Error);
                        requestObject.Description = "Authenticate : User is unauthorized";         
                        requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                        return false;
                    }
                }
                else
                {
                    logger.AddtoLog(requestObject.Id, "Authenticate : Non-exists user.", LoggerLevel.Error);
                    requestObject.Description = "Authenticate : Non-exists user.";         
                    requestObject.Error_Code = SystemErrorCodes.Client_No_Permission;
                    return false;
                }
            }
            else
            {
                logger.AddtoLog(requestObject.Id,"Client IP address is unauthorized", LoggerLevel.Error);
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
        public bool ValidateCommand(string systemCommandRequested, ref RequestObject requestObject)
        {

            if (requestObject.Equals(null))
                requestObject = new RequestObject();

            string[] commandParts = systemCommandRequested.Split(' ');
            string commandName = commandParts[0].ToUpper().Trim();
            int msisdnIndex = 1; // default first params is isdn it canbe change base on element's system command.

            // start to validate the command to element server
            List<NameValuePair> nameValuePair = new List<NameValuePair>();

            // check system command name in the database
            if (!(this.SystemCommands.ContainsKey(commandName)))
            {
                requestObject.Error_Code = Asiacell.ITADLibraries_v1.Utilities.SystemErrorCodes.Client_Unknow_Command;
                requestObject.Description = "ValidateCommand : Unknow system command.";         
                logger.AddtoLog(requestObject.Id,"ValidateCommand : Unknow system command.", LoggerLevel.Error);
                return false;
            }

            // command is exists check params validation
            string[] commandParams = this.SystemCommands[commandName].CommandParam.Split(',');
            
            // pair number of parameters
            if (!(commandParams.Length == commandParts.Length - 1))
            {
                requestObject.Error_Code = SystemErrorCodes.Client_Invalid_Parameter;
                requestObject.Description = "ValidateCommand : Mismatch parameters.";                
                logger.AddtoLog(requestObject.Id, "ValidateCommand : Mismatch parameters.", LoggerLevel.Error);
                return false;
            }
            
            // check MSSIDN or IMSI and gather into key pair parameter values
            if (commandParts.Length > 1)
            {
                // check through to find the msdn parameters to move to the first param of the params list.
                //
                for (int i = 0; i < commandParams.Length; i++)
                {
                    string p = commandParams[i];

                    if (p.ToLower().Equals("%isdn%") || p.ToLower().Equals("%msisdn%") || p.ToLower().Equals("%imsi%"))
                    {
                        msisdnIndex = i + 1;

                        string firstParam = commandParts[msisdnIndex].Trim();

                        // check the first parameter value must be MSISDN or IMSI
                        if (Functions.IsMSISDN(firstParam) || Functions.IsIMSI(firstParam))
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
                        nameValuePair.Add(new NameValuePair { Name = commandParams[i - 1], Value = commandParts[i].Trim() });
                    }
                }
            }

            try
            {
                requestObject.Command_Name = this.SystemCommands[commandName].CommandName;
                requestObject.CommandID = this.SystemCommands[commandName].SystemCommandId;
                requestObject.Element_Id = this.SystemCommands[commandName].ElementTypeId;
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
        /// Authenticate with command
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="systemCommand"></param>
        /// <returns></returns>
        public bool Authenticate(string username, string password, string systemCommand, ref RequestObject requestObject)
        {
            if (requestObject.Equals(null))
                requestObject = new RequestObject();
             

            // if you authentication is success
            bool result = this.Authenticate(username, password, ref requestObject);

            // validate command
            if (result == true)
            {
                result = this.ValidateCommand(systemCommand, ref requestObject);

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
