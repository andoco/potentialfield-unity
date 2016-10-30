namespace Andoco.Unity.Framework.Sensors
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;
    using UnityEngine;

    /// <summary>
    /// Action to cleanup the detected objects in the <see cref="DetectionCache"/>.
    /// </summary>
    public class DetectionCleanup : ActionTask
    {
        public DetectionCleanup(ITaskIdBuilder id)
            : base(id)
        {
        }

        /// <summary>
        /// If true, removes 
        /// </summary>
        public bool All { get; set; }

        /// <summary>
        /// If true, clears the record of detections that have come to an end since the last cleanup.
        /// </summary>
        public bool Ended { get; set; }

        /// <summary>
        /// If true, clears the record of detections that have started since the last cleanup.
        /// </summary>
        public bool Started { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var detectionCache = node.Context.GetGameObject().GetComponent<DetectionCache>();

            detectionCache.Cleanup(this.Started || this.All, this.Ended || this.All);

            return TaskResult.Success;
        }
    }
}
