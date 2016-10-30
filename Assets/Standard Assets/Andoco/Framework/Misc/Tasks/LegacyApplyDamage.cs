using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine.Assertions;
using Andoco.Core.Pooling;

namespace Andoco.Unity.Framework.Misc
{
    public class LegacyApplyDamage : ActionTask
    {
        public LegacyApplyDamage(ITaskIdBuilder id)
            : base(id)
        {
            this.Profile = "default";
        }

        public string Group { get; set; }

        public string Profile { get; set; }

        protected override TaskState CreateState()
        {
            return new State();
        }

        protected override void Initializing(ITaskNode node)
        {
            var state = (State)node.State;
            var go = node.Context.GetGameObject();
            state.groups = go.GetComponent<GameObjectGroup>();
            state.damage = go.GetComponent<Damage>();
        }

        public override TaskResult Run(ITaskNode node)
        {
            Assert.IsFalse(string.IsNullOrEmpty(this.Group));

            var state = (State)node.State;

            var targets = state.groups.GetInGroup(this.Group);

            for (int i = 0; i < targets.Count; i++)
            {
                state.damage.DamageTarget(targets[i], this.Profile);
            }

            targets.ReturnToPool();

            return TaskResult.Success;
        }

        private class State : TaskState
        {
            public GameObjectGroup groups;
            public Damage damage;
        }
    }
}