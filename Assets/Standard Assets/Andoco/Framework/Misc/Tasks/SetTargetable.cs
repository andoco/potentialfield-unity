using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;

namespace Andoco.Unity.Framework.Misc
{
    public class SetTargetable : ActionTask
    {
        public SetTargetable(ITaskIdBuilder id)
            : base(id)
        {
        }

        public bool Value { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var targetable = node.Context.GetGameObject().GetComponent<Targetable>();

            targetable.isTargetable = this.Value;

            return TaskResult.Success;
        }
    }
}
