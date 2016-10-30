namespace Andoco.Unity.Framework.Misc
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// Holds potential targets in a queue, and allows the targets to be pulled from the queue or delivered on a schedule.
    /// </summary>
    public class TargetQueue : MonoBehaviour
    {
        private List<Transform> queue = new List<Transform>();

        void Recycled()
        {
            this.queue.Clear();
        }

        public void Enqueue(Transform target)
        {
            this.queue.Add(target);
        }

        public void EnqueueUnique(Transform target)
        {
            if (!this.queue.Contains(target))
            {
                this.Enqueue(target);
            }
        }

        public Transform Dequeue(Transform target = null)
        {
            Transform dequeuedTx;

            this.TryDequeue(out dequeuedTx, target);

            return dequeuedTx;
        }

        public bool IsQueued(Transform target)
        {
            Assert.IsNotNull(target);

            return this.queue.Contains(target);
        }

        public bool TryDequeue(out Transform dequeuedTx, Transform target = null)
        {
            dequeuedTx = null;

            if (this.queue.Count == 0)
                return false;

            if (target != null)
            {
                var index = this.queue.IndexOf(target);

                if (index >= 0)
                {
                    this.queue.RemoveAt(index);
                    dequeuedTx = target;
                    return true;
                }

                // Desired target not in queue so return nothing.
                return false;
            }

            // Pick the first non-destroyed target.
            while (this.queue.Count > 0 && dequeuedTx == null)
            {
                dequeuedTx = this.queue[0];
                this.queue.RemoveAt(0);
            }

            return dequeuedTx != null;
        }
    }
}
