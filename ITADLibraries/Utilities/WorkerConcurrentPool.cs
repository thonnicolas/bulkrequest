using System;
using Asiacell.ITADLibraries.Utilities;
using Asiacell.ITADLibraries.LibDatabase;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Asiacell.ITADLibraries.LibLogger;

namespace Asiacell.ITADLibraries.Utilities
{


    public class WorkerConcurrentPool : IDisposable
    {
        private BlockingCollection<object> idleworker;
        private IConnectionControllerPool[] Pool = null;
        private bool isStop = false; 
        
        
        /// <summary>
        /// The contractor of Pool to call the executeable task and manage Pool size
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="db"></param>
        public WorkerConcurrentPool(int MaxPoolSize, IFactoryCreator factor)
        {
            Initialize(MaxPoolSize, factor);
        }


        private void Initialize(int MaxPoolSize, IFactoryCreator factory)
        {
            int NumberOfTask = Math.Max(1, MaxPoolSize);
            idleworker = new BlockingCollection<object>(boundedCapacity: NumberOfTask);
            Pool = new IConnectionControllerPool[NumberOfTask];
            Parallel.For(0, NumberOfTask, ind => { Pool[ind] = factory.Create(idleworker); });
        }

        /// <summary>
        /// Execute Task
        /// </summary>
        /// <param name="Command"></param>
        public void Execute(object businessObject)
        {
            if (!isStop)
            {
                IConnectionControllerPool task = (IConnectionControllerPool)idleworker.Take();
                task.Process(businessObject);
            }
            else
            {
                throw new Exception("The worker pool is already stoped.");
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
                IConnectionControllerPool task = (IConnectionControllerPool)idleworker.Take();
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
                IConnectionControllerPool task = (IConnectionControllerPool)idleworker.Take();
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
            return this.idleworker.Count;
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
