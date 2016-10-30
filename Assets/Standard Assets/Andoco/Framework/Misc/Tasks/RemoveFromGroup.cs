using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Misc
{
    public class RemoveFromGroup : ActionTask
    {
        private readonly GameObjectGroup groups;

        public RemoveFromGroup(ITaskIdBuilder id, GameObjectGroup groups)
            : base(id)
        {
            this.groups = groups;
        }

        public string Group { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            Assert.IsFalse(string.IsNullOrEmpty(this.Group));

            var go = node.Context.GetGameObject();

            this.groups.Remove(go, this.Group);

            return TaskResult.Success;
        }
    }
}
