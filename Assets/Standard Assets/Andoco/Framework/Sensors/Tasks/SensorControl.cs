namespace Andoco.Unity.Framework.Sensors
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class SensorControl : ActionTask
    {
        public SensorControl(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Name { get; set; }

        public ActionKind Action { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            Assert.IsFalse(string.IsNullOrEmpty(this.Name));

            var go = node.Context.GetGameObject();

            // TODO: Improve sensor discovery and cache in state.
            var sensorTx = go.transform.Find(this.Name);
            Assert.IsNotNull(sensorTx);

            switch (this.Action)
            {
                case ActionKind.Activate:
                    sensorTx.gameObject.SetActive(true);
                    break;
                case ActionKind.Deactivate:
                    sensorTx.gameObject.SetActive(false);
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unsupported {0} value {1}", typeof(ActionKind), this.Action));
            }

            return TaskResult.Success;
        }

        public enum ActionKind
        {
            None,
            Activate,
            Deactivate
        }
    }
}

