using System;
using System.Threading;
using System.Threading.Tasks;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibLogger;
using System.Collections.Concurrent;
using Asiacell.ITADLibWCF.Business;
using Asiacell.ITADLibraries.LibDatabase;
using System.Collections.Generic;

namespace Asiacell.ITADLibWCF.Classes
{
    /// <summary>
    /// Singleton ElementObject class
    /// </summary>
    public sealed class ElementObject : ElementObjectAbstract
    {
        // Singleton instance
        private static volatile ElementObject _instance;
        // safe thread for initialize instance of the class
        public static object syncRoot = new Object();
        private bool _isStopScheduler { get; set; }
        private Task _scheduler;
        static private int Interval = 100;
        private LoggerEntities logger = null;

        # region Properties
        public ConcurrentDictionary<int, GSMCommand> GSMCommands
        {
            get
            {
                return this._gsmCommands;
            }
        }
        public ConcurrentDictionary<int, RangeInfo> RangeInfos
        {
            get
            {
                return this._rangeInfos;
            }
        }
        public ConcurrentDictionary<string, ElementLogin> ElementLogins
        {
            get
            {
                return this._elementLogins;
            }
        }

        # endregion

        /// <summary>
        /// Intial objects
        /// </summary>
        /// <param name="Interval"></param>
        /// <param name="logger"></param>
        private void Initialize(int Interval, LoggerEntities logger, int elementTypeId)
        {
            this.logger = logger;
            ElementObject.Interval = Interval;

            if (elementTypeId > 0)
            {
                this._elementTypeId = elementTypeId;
            }

            lock (this)
            {
                this.SyncWithDB();
            }

            // Start Thread Here
            //this._isStopScheduler = false;

            //_scheduler = new Task(this.SyncScheduler);
            //_scheduler.Start();
        }


        /// <summary>
        /// Constructor of Element object
        /// </summary>
        /// <param name="users"></param>
        /// <param name="clientIps"></param>
        private ElementObject(DBConnection db, int Interval, LoggerEntities logger)
            : base(logger, db)
        {
            // Code that runs on application startup
            Initialize(Interval, logger, 0);
           
        }


        /// Constructor of Element object
        /// </summary>
        /// <param name="users"></param>
        /// <param name="clientIps"></param>
        private ElementObject(DBConnection db, int Interval, LoggerEntities logger, int elementTypeId)
            : base(logger, db, elementTypeId)
        {
            // Code that runs on application startup
            Initialize(Interval, logger, elementTypeId);

        }

        /// <summary>
        /// Get Element object instance
        /// </summary>
        public static ElementObject Instance(LoggerEntities logger, int interval, DBConnection db)
        {
            if (_instance == null)
            {
                logger.AddtoLog("Check singleton element object", LoggerLevel.Info);
                lock (syncRoot) // thread safe
                {
                    if (_instance == null)
                        _instance = new ElementObject(db ,Interval,logger);
                }
            }

            return _instance;
        }

        /// <summary>
        /// Get Element object instance
        /// </summary>
        public static ElementObject Instance(LoggerEntities logger, int interval, DBConnection db, int elementTypeId)
        {
                     
            if (_instance == null)
            {
                lock (syncRoot) // thread safe
                {
                    if (_instance == null)
                        _instance = new ElementObject(db, Interval, logger,elementTypeId);
                }
            }

            return _instance;
           
        }

        /// <summary>
        /// Stope Scheduler
        /// </summary>
        public void StopScheduler()
        {
            this._isStopScheduler = true;
        }

        /// <summary>
        /// Sync database scheduler
        /// </summary>
        private void SyncScheduler()
        {
             
            DateTime currentDate = DateTime.Now;
            DateTime runDate = DateTime.Now.AddSeconds(Interval);

            while (!(this._isStopScheduler))
            {
                Thread.Sleep(Interval * 1000);
                // exit the thread block                
                // execute the sync database process
                lock (this)
                {
                    this.SyncWithDB();
                }
                // reprocess every seconds
                
            }
            // end of the scheduling block
        }

        /// <summary>
        /// Request command validation
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="requestJson"></param>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public bool Authenticate(string loginKey, string requestJson, ref RequestObject requestObject)
        {
            return ExecuteAuthentication(loginKey, requestJson, ref requestObject);
        }

        /// <summary>
        /// Request command validation
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="requestJson"></param>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public bool Authenticate(string loginKey, ref RequestObject requestObject)
        {
            return ExecuteAuthentication(loginKey, "", ref requestObject);
        }

        /// <summary>
        /// Authenticate element login
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="requestObject"></param>
        /// <param name="maxSession"></param>
        /// <returns></returns>
        public bool Authenticate(string loginKey, ref RequestObject requestObject, out Int32 maxSession)
        {
            return ExecuteAuthentication(loginKey, "", ref requestObject, out maxSession);
        }

        /// <summary>
        /// Authenticate element login
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="requestObject"></param>
        /// <param name="maxSession"></param>
        /// <returns></returns>
        public bool Authenticate(string loginKey, ref RequestObject requestObject, out Int32 maxSession, out Int32 sessionControlTimeout)
        {            
            return ExecuteAuthentication(loginKey, "", ref requestObject, out maxSession, out sessionControlTimeout);
        }
        /// <summary>
        /// Execute Authentication
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="requestJson"></param>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        private bool ExecuteAuthentication(string loginKey, string requestJson, ref RequestObject requestObject)
        {
            Int32 maxSession = 0;
             return ExecuteAuthentication(loginKey, requestJson, ref requestObject, out maxSession);
        }

        /// <summary>
        /// Execute Authentication
        /// </summary>
        /// <param name="loginKey"></param>
        /// <param name="requestJson"></param>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        private bool ExecuteAuthentication(string loginKey, string requestJson, ref RequestObject requestObject, out Int32 maxSession)
        {
            Int32 sessionControlTimeout = 0;
            return ExecuteAuthentication(loginKey, requestJson, ref requestObject, out maxSession, out sessionControlTimeout);
        }


        private bool ExecuteAuthentication(string loginKey, string requestJson, ref RequestObject requestObject, out Int32 maxSession, out Int32 sessionControlTimeout)
        {
            maxSession = 0;
            sessionControlTimeout = 0;
            // process result
            bool result = false;

            // deserialize the message
            //requestObject = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestObject>(requestJson);

            // check if the key is exists allow to call
            if (this._elementLogins.ContainsKey(loginKey))
            {
                ElementLogin elementLogin = this._elementLogins[loginKey];
                // if it is contain the key then compare the id                
                if (requestObject.Element_Id == this._elementLogins[loginKey].ElementTypeID)
                {
                    requestObject.Element_Name = elementLogin.Name;
                    maxSession = elementLogin.MaxSession;
                    sessionControlTimeout = elementLogin.SessionControlTimeout;
                    result = true;
                }
                else
                {
                    logger.AddtoLog(requestObject.Id, "Authenticate Failed : ElementID Not Found", LoggerLevel.Error);
                    requestObject.Description = "Authenticate Failed : ElementID Not Found";
                    requestObject.Error_Code = SystemErrorCodes.Server_ElementID_Not_Found;
                    result = false;
                }
            }
            else
            {
                logger.AddtoLog(requestObject.Id, "Authenticate Failed : Invalid login key.", LoggerLevel.Error);
                requestObject.Description = "Authenticate Failed : Invalid login key.";
                requestObject.Error_Code = SystemErrorCodes.Server_Invalid_LoginKey;
                result = false;
            }
            //requestObject.Respond_Date = DateTime.Now;
            return result;
        }

    }
}
