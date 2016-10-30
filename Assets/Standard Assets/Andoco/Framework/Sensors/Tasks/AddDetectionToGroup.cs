using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using Andoco.Unity.Framework.Misc;
using UnityEngine;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Sensors
{
    public class AddDetectionToGroup : ActionTask
    {
        public AddDetectionToGroup(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Group { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            Assert.IsFalse(string.IsNullOrEmpty(this.Group));

            var go = node.Context.GetGameObject();

            var detection = go.GetComponent<DetectionCache>();
            Assert.IsNotNull(detection);

            var groups = go.GetComponent<GameObjectGroup>();
            Assert.IsNotNull(groups);

            if (detection.StartedDetections.Count == 0)
                return TaskResult.Failure;

            var detectedGo = detection.StartedDetections[0];

            groups.Add(detectedGo, this.Group);

            return TaskResult.Success;
        }
    }
}
