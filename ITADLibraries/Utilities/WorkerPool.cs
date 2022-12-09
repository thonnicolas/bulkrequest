using System;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibDatabase;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.Utilities
{


    public class WorkerPool : IDisposable
    {
        private BlockingCollection<object> idleworkers;
        private IConnectionControllerPool[] Pool = null;
        private bool isStop = false;
        private String sessionStamp = "";
        private int currentRequestCount;
        private int tps = 0;

        /// <summary>
        /// The contractor of Pool to call the executeable task and manage Pool size
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="db"></param>
        public WorkerPool(int MaxPoolSize, IFactoryCreator factor)
        {
            Initialize(MaxPoolSize, factor, MaxPoolSize);
        }

        public WorkerPool(int MaxPoolSize, IFactoryCreator factor, int tps)
        {
            Initialize(MaxPoolSize, factor, tps);
        }


        private void Initialize(int MaxPoolSize, IFactoryCreator factory, int tps)
        {
            int NumberOfTask = Math.Max(1, MaxPoolSize);
            idleworkers = new BlockingCollection<object>(boundedCapacity: NumberOfTask);
            this.tps = tps;
            Pool = new IConnectionControllerPool[NumberOfTask];
            //Parallel.For(0, NumberOfTask, ind => { Pool[ind] = factory.Create(idleworkers); });

            for (int ind = 0; ind < NumberOfTask; ind++)
            {
                Pool[ind] = factory.Create(idleworkers);
            }
        }

        /// <summary>
        /// Execute Task
        /// </summary>
        /// <param name="Command"></param>
        public void Execute(object businessObject)
        {
            if (!isStop)
            {
                // increase request
                ManageTpsProcess();
                IConnectionControllerPool task = (IConnectionControllerPool)idleworkers.Take();                
                task.Process(businessObject);
                //System.Console.WriteLine("After Process");
            }
            else
            {
                throw new Exception("The worker pool is already stoped.");
            }
        }

        /**
         * Control TPS
         */
        private void ManageTpsProcess()
        {
            Interlocked.Increment(ref currentRequestCount);

            DateTime dateStamp = DateTime.Now;
            String latestSessionStamp = dateStamp.ToString("yyyyMMddHmmss");

            while (true)
            {

                // when set current Request to null?
                // 
                if (sessionStamp.CompareTo(latestSessionStamp) == 0 && currentRequestCount >= this.tps && this.tps != 0)
                {
                    //System.Console.WriteLine(String.Format("latestSessionStamp {0} {1} {2} {3}", sessionStamp, latestSessionStamp, currentRequestCount, idleworkers.Count));
                    //System.Console.WriteLine("Exceed");
                    Thread.Sleep(100);
                }
                else if (latestSessionStamp.CompareTo(sessionStamp) != 0 && !String.IsNullOrEmpty(sessionStamp))
                {
                    //System.Console.WriteLine("Different");
                    Interlocked.Exchange(ref currentRequestCount, 0);
                    Interlocked.Exchange<String>(ref sessionStamp, latestSessionStamp);
                    break;
                }
                else
                {
                    //System.Console.WriteLine("OK");
                    Interlocked.Exchange<String>(ref sessionStamp, latestSessionStamp);
                    break;
                }
                dateStamp = DateTime.Now;
                latestSessionStamp = dateStamp.ToString("yyyyMMddHmmss");
            }
            
        }

        /// <summary>
        /// Execute Task with throw exeception.
        /// </summary>
        /// <param name="Command"></param>
        public void Execute(object businessObject,ref bool isAdding)
        {
            if (!isStop)
            {
                ManageTpsProcess();
                IConnectionControllerPool task = (IConnectionControllerPool)idleworkers.Take();
                task.Process(businessObject);
                isAdding = true;
            }
            else
            {
                isAdding = false;
            }
        }

        /// <summary>
        /// Execute Task
        /// </summary>
        /// <param name="Command"></param>
        public void Execute(object commandLoginObject, ExecuteCallDelegate callDelegate)
        {
            if (!isStop)
            {
                ManageTpsProcess();
                IConnectionControllerPool task = (IConnectionControllerPool)idleworkers.Take();
                task.Process(commandLoginObject, callDelegate);
            }
            else
            {
                throw new Exception("The worker pool is already stoped.");
            }

        }

        /// <summary>
        /// Count idle worker
        /// </summary>
        /// <returns></returns>
        public int GetIdleCount()
        {
            return this.idleworkers.Count;
        }
        /// <summary>
        /// Stop All task
        /// </summary>
        public void StopTask()
        {

            Parallel.For(0, Pool.Length, i =>
                {
                    try
                    {
                        Pool[i].Dispose();
                        Thread.Sleep(10);
                    }
                    catch (ThreadInterruptedException ex)
                    {
                        //Reset task
                        //Pool[i].GetThreadWorker.Interrupt();
                    }
                    catch (Exception ex)
                    {
                        //Reset task
                        //Pool[i].GetThreadWorker.Interrupt();
                    }
                });
        }

        #region IDisposable Members

        public void Dispose()
        {
            isStop = true;
            Thread.Sleep(100);

            StopTask();

            
            //try { Thread.Sleep(100); }
            //catch (ThreadInterruptedException ex) { }

            //for (int i = 0; i < Pool.Length; i++)
            //{
            //    try
            //    {
            //        Pool[i].Dispose();
            //    }
            //    catch { };

            //    //if (Pool[i].IsAlive) Pool[i].Dispose();
            //}

            //try { Thread.Sleep(100); }
            //catch (ThreadInterruptedException ex) { }

        }

        #endregion
    }
}
