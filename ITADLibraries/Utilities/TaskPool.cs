using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.Utilities
{
    // Declare a delegate type for processing a book:
    public delegate void ExecuteCallDelegate(object requestObject);

    public abstract class TaskPool : IDisposable, IConnectionControllerPool
    {
        private LoggerEntities logger;

        private static int NextWorkerID = 1;
        private int WorkerID = 1;

        //Synchronized Thread
        //object locker = new object();

        //private QueueObject task;
        //private QueueObject IdleWorker;

        //private BlockingCollection<object> task = null;
        private ConcurrentBag<object> task = null;
        private BlockingCollection<object> idleWorkers = null;
        private ExecuteCallDelegate callDelegate;

        private volatile bool IsRequestStop;

        /// <summary>
        /// Create Internal Task Thread
        /// </summary>
        private Task InternalTask;
        private CancellationTokenSource canceller;


        public object GetObject()
        {
            object item;
            
            if (task.TryTake(out item)) return item;

            return null;
        }

        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="idleWorkers"></param>
        public TaskPool(BlockingCollection<object> idleWorkers, LoggerEntities logger)
        {
            Initialize(idleWorkers, logger);
        }


        /// <summary>
        /// Contractor
        /// </summary>
        /// <param name="IdleWorker"></param>
        public TaskPool(BlockingCollection<object> IdleWorker)
        {
            LoggerEntities logger = new LoggerEntities();
            Initialize(IdleWorker, logger);
        }

        private void Initialize(BlockingCollection<object> idleWorkers, LoggerEntities logger)
        {
            this.logger = logger;
            this.idleWorkers = idleWorkers;
            this.idleWorkers.Add(this);
            //task = new BlockingCollection<object>(boundedCapacity: 1);
            task = new ConcurrentBag<object>();

            WorkerID = GetNextWorkerID();           

            //Thread.Sleep(10);

            canceller = new CancellationTokenSource();
            /*
            InternalTask = new Thread(StartWork);
            InternalTask.Name = "Task-" + WorkerID.ToString().PadLeft(3, '0');
            InternalTask.Start();
             */
            StartWork();
        }

        //public string GetMessageWithID(string Message)
        //{
        //    return String.Format("Task ID : {0} - {1}", WorkerID, Message);
        //}

        /// <summary>
        /// Internal Thread Worker
        /// </summary>
        private void StartWork()
        {
            CancellationToken token = canceller.Token;
            InternalTask = Task.Factory.StartNew(() =>
            {
                while (!IsRequestStop)
                {
                    try
                    {
                        //logger.AddtoLog("Waiting task to execute.......", LoggerLevel.Info);

                        if (token.IsCancellationRequested) break;
                        
                        //if no task, then wait for new task(s)
                        //object getTask = task.Take();
                        object getTask = GetObject();
                        
                        //if task=null, then we need to exit->Terminate service
                        if (getTask == null)
                        {
                            //System.Console.WriteLine("Sleep 1 second for no task to execute");
                            Thread.Sleep(500);
                        }
                        else { 
                            //Process task
                            object obj = RunWork(getTask);
                            if (callDelegate != null) callDelegate(obj);

                            //Add to idle queue
                            idleWorkers.Add(this);
                        }
                    }
                    catch (ThreadInterruptedException ex)
                    {
                        logger.AddtoLog("The task is interrupted : " + ex.Message, LoggerLevel.Error);
                    }
                    catch (Exception ex)
                    {
                        logger.AddtoLog(ex.Message, ex, LoggerLevel.Error);
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Adding task objec with delegation ref object
        /// </summary>
        /// <param name="Command"></param>
        public void Process(object Command, ExecuteCallDelegate callDelegate)
        {
            if (!IsRequestStop)
            {
                task.Add(Command);
            }

            this.callDelegate = callDelegate;
        }

        /// <summary>
        /// Add task object without delegation object
        /// </summary>
        /// <param name="Command"></param>
        public void Process(object Command)
        {
            //Thread.Sleep(TimeSpan.FromMilliseconds(1));
            if (!IsRequestStop)
            {
                task.Add(Command);
            }

            this.callDelegate = null;
        }


        /// <summary>
        /// Perform task requested
        /// </summary>
        /// <param name="Command"></param>
        public abstract object RunWork(object Command);


        /// <summary>
        /// Get Worker ID
        /// </summary>
        public int GetWorkerID { get { return WorkerID; } }

        /// <summary>
        /// Get Worker ID
        /// </summary>
        /// <returns></returns>
        public int GetNextWorkerID()
        {
            int id = NextWorkerID;
            NextWorkerID++;
            return id;
        }

        /// <summary>
        /// Is Thread Alive
        /// </summary>
        //public bool IsAlive { get { return InternalTask.Status } }

        /// <summary>
        /// Return Internal Thread
        /// </summary>
        public Task GetThreadWorker { get { return InternalTask; } }

        #region IDisposable Members

        public virtual void Dispose()
        {

            IsRequestStop = true;
            
            logger.AddtoLog("Reqeust to stop process.... please wait ", LoggerLevel.Info);
            {
                //Set Null to exit internal Thread                   
                canceller.Cancel();                
                //task.Count
                task.Add(null);                
                InternalTask.Wait(60000);
                //InternalTask.Join(TimeSpan.FromSeconds(20));
            }

            logger.AddtoLog("The task is stoped successful", LoggerLevel.Info);

        }

        #endregion
    }
}
