using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration;
using Asiacell.ITADLibraries.LibLogger;
using Asiacell.ITADLibraries.LibDatabase;
using Asiacell.ITADLibraries.Utilities;
using BulkRequestConsole.Classes;
using System.Threading;
using System.Threading.Tasks;
using BulkRequestConsole.Utilities;

namespace BulkRequestConsole
{
    partial class BulkRequestService : ServiceBase
    {
        //public int bulkInsert = 10;
        //public static volatile bool isStopped = false;
        //LoggerEntities logger;
        //DBConnection db;
        //CollectionManager collector;
        //CancellationTokenSource canceller;
        //CancellationToken token;
        //Task t = null;

        public BulkRequestService()
        {
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            ApplicationInitialization();

        }
         

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            Releases();
        }

        private static void ApplicationInitialization()
        {



            // for openning multiple connectivity to the server concurrently
            System.Net.ServicePointManager.DefaultConnectionLimit = Int32.MaxValue;

            int isScheudle = Convert.ToInt32(ConfigurationManager.AppSettings["IsSchedule"]);
            int testCount = Convert.ToInt32(ConfigurationManager.AppSettings["TestCount"]);
            int isSameSchedule = Convert.ToInt32(ConfigurationManager.AppSettings["IsSameSchedule"]);
            int ScheduleInterval = Convert.ToInt32(ConfigurationManager.AppSettings["ScheduleInterval"]);
            int ElementTypeId = Convert.ToInt32(ConfigurationManager.AppSettings["ElementTypeID"]);
            // 10: Ema, 11: AIR, 12: Server, 13: Clear Session, 14: Test WCF Service, 15: HxC, 16: BlackBox, 17: Test DB Logger
            int NumberOfClient = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfClient"]);
            string emaSessionPath = Convert.ToString(ConfigurationManager.AppSettings["EmaSessionPath"]);
            int numberOfRequestPerClient = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfRequestPerClient"]);
            int HttpTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["HttpTimeout"]);
            bool HttpKeepAlive = Convert.ToBoolean(ConfigurationManager.AppSettings["HttpKeepAlive"]);
            string serviceUrl = System.Configuration.ConfigurationManager.AppSettings["ServiceServerUrl"].ToString();
            string ResultOutPath = ConfigurationManager.AppSettings["ResultOutPath"].ToString();
            string SampleTestCaseCommandRequest = ConfigurationManager.AppSettings["SampleTestCaseCommandRequest"].ToString();

            GlobalObject.RechargeLogCommands = ConfigurationManager.AppSettings["RechargeLogCommands"].ToString();
            GlobalObject.SignalRConnnectionURL = Functions.ToString(ConfigurationManager.AppSettings["SignalRConnnectionURL"]);


            GlobalObject.logger = new LoggerEntities();
            try
            {
                GlobalObject.db = new DBConnection(ConfigurationManager.ConnectionStrings["itmdw_ConnString"].ConnectionString, GlobalObject.logger);
                GlobalObject.bulkInsert = Convert.ToInt32(ConfigurationManager.AppSettings["DBTransactionBulkInsert"]);
                string GWUser = ConfigurationManager.AppSettings["GWUser"].ToString();
                string GWPassword = ConfigurationManager.AppSettings["GWPassword"].ToString();
                string migrate_table = ConfigurationManager.AppSettings["migrate_table"].ToString();
                string ApplicationID = ConfigurationManager.AppSettings["ApplicationID"].ToString();
                GlobalObject.logger.AddtoLog("Starting the Bulk Execution service", LoggerLevel.Info);
                if (GlobalObject.db.StartupConnection())
                {
                    AutomateApplication(ApplicationID, ElementTypeId, isScheudle, ScheduleInterval, GlobalObject.db, GlobalObject.logger, serviceUrl, GWUser, GWPassword, migrate_table, NumberOfClient, numberOfRequestPerClient, isSameSchedule, HttpKeepAlive, HttpTimeout, testCount); // no schedule
                }
                else
                {
                    GlobalObject.logger.AddtoLog("Bulk Execution service could not start due problem of database connectivity", LoggerLevel.Error);
                }
            }
            catch (Exception e)
            {
                GlobalObject.logger.AddtoLog(e, LoggerLevel.Error);
                GlobalObject.logger.AddtoLog("Bulk Execution service could not start due problem of database connectivity", LoggerLevel.Error);
            }

        }

        private static void AutomateApplication(string ApplicationID, int elementId, int isRunSchedule, int intervalSchedule, DBConnection db, LoggerEntities logger, string serviceUrl, string GWUser, string GWPassword, string migrate_table,
            int numWorker = 5,
            int numRequestPerClient = 1,
            int isSameSchedule = 1, bool HttpKeepAlive = true, int HttpTimeout = 6000, int testCount = 2) // minutes
        {

            IFactoryCreator factory = null;

            if (elementId == 20)
            {
                BatchProcessingFactory groupRechargeFactory = new BatchProcessingFactory(db, logger);
                //create factory here
                factory = new BulkRequestPoolFactory(logger, serviceUrl, db, GWUser, GWPassword, migrate_table, groupRechargeFactory);

            }

            WorkerPool[] worker = new WorkerPool[numWorker];

            for (int i = 0; i < numWorker; i++)
            {
                worker[i] = new WorkerPool(numRequestPerClient, factory); //  
            }

            Thread.Sleep(5000); // sleep ten seconds

            GlobalObject.collector = new CollectionManager(logger, ApplicationID, numRequestPerClient, worker, "", GWUser, GWPassword, db, db, migrate_table);


            GlobalObject.canceller = new CancellationTokenSource();
            GlobalObject.token = GlobalObject.canceller.Token;


            GlobalObject.taskJob = Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    Thread.Sleep(intervalSchedule * 1000);

                    if (GlobalObject.canceller.IsCancellationRequested == true || GlobalObject.taskJob.IsCompleted == true)
                    {
                        logger.AddtoLog("Service stopped successfully", LoggerLevel.Info);
                        Console.WriteLine("Stopped success");
                        break;
                    }

                    // running scheduling here
                    //Console.WriteLine("Run!!!");
                    GlobalObject.collector.GetNonRequested();
                }
            }, GlobalObject.token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            //Console.WriteLine("Press ENTER key to stop the application");
           // Console.ReadLine();
            //Console.WriteLine(DateTime.Now.ToString("hh24:mi:ss.ffff") + " : Stopping application");
            //Releases();
        }

        private static void Releases()
        {
            Console.WriteLine(DateTime.Now.ToString("hh24:mi:ss.ffff") + " : Stopping application");
            GlobalObject.canceller.Cancel();
            GlobalObject.taskJob.Wait();
            Console.WriteLine(DateTime.Now.ToString("hh24:mi:ss.ffff") + " : Stopped application");
            try { GlobalObject.collector.Dispose(); }
            catch { }
            try { GlobalObject.db.Dispose(); }
            catch { }
            try { GlobalObject.logger.Dispose(); }
            catch { }
            Console.WriteLine(DateTime.Now.ToString("hh24:mi:ss.ffff") + " : Application completely stopped");
            //Console.ReadLine();
        }

        public static void Main(string[] args)
        {
            ServiceBase[] runService = new ServiceBase[] { new BulkRequestService() };
            ServiceBase.Run(runService);
        }
    }
}
