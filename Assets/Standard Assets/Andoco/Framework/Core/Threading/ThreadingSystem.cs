namespace Andoco.Unity.Framework.Core
{
    using System;
    using UnityEngine;

    public class ThreadingSystem : MonoBehaviour, IThreadingSystem
    {
        private ITaskQueue taskQueue;
        private IDispatcher dispatcher;

        #region Lifecycle

        void Awake()
        {
            this.dispatcher = new Dispatcher();
            this.taskQueue = new TaskQueue(this.dispatcher);
        }

        void Start()
        {
            this.taskQueue.Start();
        }

        void OnDestroy()
        {
            this.taskQueue.Stop();
        }

        #endregion

        public void Schedule(Action work, Action callback = null)
        {
            this.taskQueue.Schedule(work, callback);
        }

        public void ScheduleMain(Action work)
        {
            this.dispatcher.Schedule(work);
        }
    }
}
