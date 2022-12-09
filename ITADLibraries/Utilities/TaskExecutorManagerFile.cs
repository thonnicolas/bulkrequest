using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace Asiacell.ITADLibraries.Utilities
{
    class TaskExecutorManagerFile : IDisposable
    {
        
        private ConcurrentQueue<object> transactionQueues = null;
        private Task task = null;
        private CancellationTokenSource taskCancellor = null;
        private ITaskExecutor executor = null;

        public ConcurrentQueue<object> TransactionQueues { get { return transactionQueues; } }

        public TaskExecutorManagerFile()
        {
            Initialize(null);
        }

        public void SetExecutor(ITaskExecutor executor)
        {
            this.executor = executor;
        }

        public TaskExecutorManagerFile(ITaskExecutor taskExecutor)
        {
            Initialize(taskExecutor);
        }

        private void Initialize(ITaskExecutor taskExecutor)
        {
            this.transactionQueues = new ConcurrentQueue<object>();
            this.taskCancellor = new CancellationTokenSource();
            this.executor = taskExecutor;
            // start the task scheduler to collect new queued items and execute
            this.StartCollector();
        }

        public void AddNewTransaction(object transactionObject)
        {
            this.transactionQueues.Enqueue(transactionObject);
        }

        [STAThread]
        private void StartCollector()
        {
            CancellationToken token = taskCancellor.Token;
            task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    // end the task loop if cancellation token is quested to cancel and log queues is empty
                    if (token.IsCancellationRequested == true && transactionQueues.IsEmpty == true)
                        break;
                    if (!transactionQueues.IsEmpty)
                    {
                        if (executor != null)
                        {
                            // check if the collection of concurrent queue is equal 100 then insert ask bulk into database
                            executor.Execute(this.transactionQueues);
                        }
                        if (task != null)
                            task.Wait(10);
                    }
                    else
                    {
                        if (task != null)
                            task.Wait(100);
                    }
                }
            }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }


        public void Dispose()
        {
            try
            {
                taskCancellor.Cancel();
            }
            catch
            { }
            try
            {
                task.Wait();
            }
            catch { }
        }
        
    }
}
