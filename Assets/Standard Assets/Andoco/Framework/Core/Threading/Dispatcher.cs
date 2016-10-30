namespace Andoco.Unity.Framework.Core
{
    using System;
    using System.Collections.Generic;
    using UnityEngine.Assertions;

    public class Dispatcher : IDispatcher
    {
        private readonly List<Action> queue = new List<Action>();
        private readonly object locker = new object();

        public void Schedule(Action action)
        {
            lock (this.locker)
            {
                this.queue.Add(action);
            }
        }

        private void Update()
        {
            lock (this.locker)
            {
                if (this.queue.Count > 0)
                {
                    var count = this.queue.Count;

                    for (int i = 0; i < count; i++)
                    {
                        Assert.IsNotNull(this.queue[i]);
                        this.queue[i]();
                    }

                    this.queue.RemoveRange(0, count);
                }
            }
        }
    }
}
