using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.BehaviorTree.Scheduler;
using Andoco.Core.Signals;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Misc
{
    public class AddToGroup : ActionTask
    {
        private readonly GameObjectGroup groups;

        public AddToGroup(ITaskIdBuilder id, GameObjectGroup groups)
            : base(id)
        {
            this.groups = groups;
        }

        public string Group { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            Assert.IsFalse(string.IsNullOrEmpty(this.Group));

            var go = node.Context.GetGameObject();

            if (!this.groups.IsInGroup(go, this.Group))
                this.groups.Add(go, this.Group);

            return TaskResult.Success;
        }
    }
}
