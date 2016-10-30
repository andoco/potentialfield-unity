namespace Andoco.Unity.Framework.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Andoco.Core;

    public sealed class BackgroundWorkQueue
    {
        private Thread thread;
        private object locker;
        private IList<WorkItem> workQueue;
        private AutoResetEvent waitHandle;

        public static BackgroundWorkQueue Instance { get; private set; }

        static BackgroundWorkQueue()
        {
            Instance = new BackgroundWorkQueue();
        }

        public BackgroundWorkQueue()
        {
            this.workQueue = new List<WorkItem>();
            this.waitHandle = new AutoResetEvent(false);
            this.locker = new object();
            this.thread = new Thread(this.ProcessWorkQueue);
            this.thread.Start();
        }

        public void Schedule(Action work)
        {
            lock (this.locker)
            {
                var workItem = new WorkItem { work = work };
                this.workQueue.Add(workItem);
                this.waitHandle.Set();
            }
        }

        private void ProcessWorkQueue(object state)
        {
            UnityEngine.Debug.LogFormat("Starting background work queue thread");

            try
            {
                while (true)
                {
                    // If the work queue is empty, wait until a work item is added.
                    if (this.workQueue.Count == 0)
                    {
                        this.waitHandle.WaitOne();
                    }

                    lock (this.locker)
                    {
                        var workItem = this.workQueue.Dequeue();
                        workItem.work();
                    }
                }
            }
            catch (Exception e)
            {
                // Make sure we log the exception because the main Unity thread won't know about it.
                UnityEngine.Debug.LogException(e);
                throw;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        private struct WorkItem
        {
            public Action work;
        }
    }
}
