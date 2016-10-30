namespace Andoco.Unity.Framework.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Andoco.Core;

    /// <summary>
    /// TaskQueue will sequentially run tasks on a background thread.
    /// </summary>
    public class TaskQueue : ITaskQueue
    {
        private readonly Thread thread;
        private readonly object locker;
        private readonly IList<WorkItem> workQueue;
        private readonly AutoResetEvent waitHandle;
        private bool exiting;
        private readonly IDispatcher dispatcher;

        public TaskQueue(IDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.workQueue = new List<WorkItem>();
            this.waitHandle = new AutoResetEvent(false);
            this.locker = new object();
            this.thread = new Thread(this.ProcessWorkQueue);
        }

        public void Start()
        {
            UnityEngine.Debug.LogFormat("Starting task queue thread {0}", this.thread.ManagedThreadId);
            this.thread.Start();
        }

        public void Stop()
        {
            UnityEngine.Debug.LogFormat("Stopping task queue thread {0}", this.thread.ManagedThreadId);
            this.exiting = true;
            this.thread.Interrupt();
        }

        public void Schedule(Action work, Action callback = null)
        {
            var workItem = new WorkItem { work = work, callback = callback };

            lock (this.locker)
            {
                this.workQueue.Add(workItem);
                this.waitHandle.Set();
            }
        }

        private void ProcessWorkQueue(object state)
        {
            UnityEngine.Debug.LogFormat("Entering task queue loop");

            try
            {
                while (!this.exiting)
                {
                    // If the work queue is empty, wait until a work item is added.
                    if (this.workQueue.Count == 0)
                    {
                        this.waitHandle.WaitOne();
                    }

                    if (this.workQueue.Count == 0)
                        continue;

                    WorkItem workItem;

                    lock (this.locker)
                    {
                        workItem = this.workQueue.Dequeue();
                    }

                    workItem.work();

                    if (workItem.callback != null)
                    {
                        this.dispatcher.Schedule(workItem.callback);
                    }
                }
            }
            catch (ThreadInterruptedException)
            {
                UnityEngine.Debug.LogFormat("Task queue thread {0} interrupted. Will exit cleanly.", this.thread.ManagedThreadId);
            }
            catch (Exception e)
            {
                // Make sure we log the exception because the main Unity thread won't know about it.
                UnityEngine.Debug.LogException(e);
                throw;
            }

            UnityEngine.Debug.Log("Task queue loop now finished.");
        }

        private struct WorkItem
        {
            public Action work;
            public Action callback;
        }
    }
}
