namespace Andoco.Unity.Framework.Misc
{
    using System;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    [Obsolete("FireController handles passing an aim target to projectile now")]
    public class Aimer : MonoBehaviour
    {
        private const int QueueHeadIndex = 0;
        private const string LogTargetKey = "target";
        private const string LogNumTargetsQueuedKey = "numTargetsQueued";

        [Inject]
        private IStructuredLog log;

        private List<object> targetQueue = new List<object>();

        public bool AnyQueued { get { return this.targetQueue.Count > 0; } }

        public int NumQueued { get { return this.targetQueue.Count; } }

        #region Lifecycle

        void FixedUpdate()
        {
            this.DequeueInvalidTargets();
        }

        void Recycled()
        {
            this.targetQueue.Clear();
        }

        #endregion

        public void Clear()
        {
            if (this.log.Status.IsTraceEnabled)
                this.log.Trace("Clearing target queue", x => x.Field(LogNumTargetsQueuedKey, this.targetQueue.Count));
            
            this.targetQueue.Clear();
        }

        public void EnqueueTarget(Transform target)
        {
            Assert.IsNotNull(target);

            this.targetQueue.Add(target);
        }

        /// <summary>
        /// Aims at the next queued target and dequeues the target.
        /// </summary>
        /// <remarks>
        /// Allows the target to be targeted a single time.
        /// </remarks>
        /// <param name="aimedTx">The transform to aim at the next queued target.</param>
        public void Aim(Transform aimedTx)
        {
            Assert.IsNotNull(aimedTx);

            var target = this.targetQueue.Count > 0 ? this.targetQueue[QueueHeadIndex] : null;

            if (target != null)
                this.targetQueue.RemoveAt(QueueHeadIndex);

            if (this.log.Status.IsDebugEnabled)
                this.log.Debug("Aiming at target", x => x
                    .Field(LogTargetKey, target)
                    .Field(LogNumTargetsQueuedKey, this.targetQueue.Count)
                );

            this.AimAtTarget(aimedTx, target);
        }

        /// <summary>
        /// Aims at the next queued target without dequeuing the target.
        /// </summary>
        /// <remarks>
        /// Allows the same target to be targeted multiple times.
        /// </remarks>
        /// <param name="aimedTx">The transform to aim at the next queued target.</param>
        public void AimPersistent(Transform aimedTx)
        {
            Assert.IsNotNull(aimedTx);

            var target = this.targetQueue.Count > 0 ? this.targetQueue[QueueHeadIndex] : null;

            if (this.log.Status.IsDebugEnabled)
                this.log.Debug("Aiming at persistent target", x => x
                    .Field(LogTargetKey, target)
                    .Field(LogNumTargetsQueuedKey, this.targetQueue.Count)
                );

            this.AimAtTarget(aimedTx, target);
        }

        public void AimAtTarget(Transform aimedTx, object target)
        {
            var txTarget = GetTargetTransform(target);

            if (txTarget != null)
            {
                var aimable = aimedTx.GetComponent<IAimable>();

                if (aimable != null)
                    aimable.Aim(txTarget);
                else
                    Debug.LogWarningFormat("Cannot aim the transform {0} because it does not have a component of type IAimable", aimedTx);
            }
        }
            
        public void RemoveTarget(Transform target)
        {
            Assert.IsNotNull(target);

            this.targetQueue.Remove(target);
        }

        private static Transform GetTargetTransform(object target)
        {
            var targetedTx = target as Transform;

            if (targetedTx != null)
                return targetedTx;

            var targetedGo = target as GameObject;

            if (targetedGo != null)
                return targetedGo.transform;

            var targetedCmp = target as Component;

            if (targetedCmp != null)
                return targetedCmp.transform;

            return null;
        }

        private void DequeueInvalidTargets()
        {
            object target = null;

            while (this.targetQueue.Count > 0 && target == null)
            {
                target = this.targetQueue[QueueHeadIndex];

                var unityObject = target as UnityEngine.Object;

                if (unityObject != null && ObjectValidator.Validate(unityObject))
                {
                    // We're reached a non-disposed and active target.
                    return;
                }

                // Discard the target.
                this.targetQueue.RemoveAt(QueueHeadIndex);
            }
        }
    }
}
