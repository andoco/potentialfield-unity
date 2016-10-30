using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Core.Pooling;
using Andoco.Unity.Framework.BehaviorTree;
using Andoco.Unity.Framework.Misc;
using UnityEngine;

namespace Andoco.Unity.Framework.Units
{
    public class RecoverStunned : ActionTask
    {
        public RecoverStunned(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Group { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var go = node.Context.GetGameObject();
            var groups = go.GetComponent<GameObjectGroup>();

            var stunned = groups.GetInGroup(this.Group);

            for (int i = 0; i < stunned.Count; i++)
            {
                var stunnable = stunned[i].GetComponent<Stunnable>();
                stunnable.Recover();
            }

            stunned.ReturnToPool();

            return TaskResult.Success;
        }
    }
}