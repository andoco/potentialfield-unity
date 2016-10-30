namespace Andoco.Unity.Framework.Misc
{
    using System;
    using System.Collections.Generic;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Conditions;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Sensors;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class IsQueuedTarget : Condition
    {
        public IsQueuedTarget(ITaskIdBuilder id)
            : base(id)
        {
        }

        protected override bool Evaluate(ITaskNode node)
        {
            var go = node.Context.GetGameObject();
            var detectionCache = go.GetComponent<DetectionCache>();
            var targetQueue = go.GetComponent<TargetQueue>();

            var detected = new List<GameObject>();
            detectionCache.Fill(detected);

            for (int i = 0; i < detected.Count; i++)
            {
                var entity = detected[i].GetComponentInParent<Entity>();

                if (targetQueue.IsQueued(entity.transform))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
