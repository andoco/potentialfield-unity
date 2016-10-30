using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField.Tasks
{
    public class PotentialControl : ActionTask
    {
        public PotentialControl(ITaskIdBuilder id)
            : base(id)
        {
        }

        public ActionKind Action { get; set; }

        public string SourceKey { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var source = node.Context.GetComponent<PotentialFieldSource>();

            switch (this.Action)
            {
                case ActionKind.None:
                    break;
                case ActionKind.Enable:
                    source.GetNodeSource(this.SourceKey).Enabled = true;
                    break;
                case ActionKind.Disable:
                    source.GetNodeSource(this.SourceKey).Enabled = false;
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown potential control action kind {0}", this.Action));
            }

            return TaskResult.Success;
        }

        public enum ActionKind
        {
            None,
            Enable,
            Disable
        }
    }
}