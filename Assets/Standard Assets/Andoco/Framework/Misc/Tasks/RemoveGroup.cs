using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Misc
{
    public class RemoveGroup : ActionTask
    {
        public RemoveGroup(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Group { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            Assert.IsFalse(string.IsNullOrEmpty(this.Group));

            var groups = node.Context.GetComponent<GameObjectGroup>();
            groups.Remove(this.Group);

            return TaskResult.Success;
        }
    }
}
