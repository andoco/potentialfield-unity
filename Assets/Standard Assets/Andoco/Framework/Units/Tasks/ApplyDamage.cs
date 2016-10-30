namespace Andoco.Unity.Framework.Units.Tasks
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Core.Pooling;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Misc;
    using UnityEngine.Assertions;

    public class ApplyDamage : ActionTask
    {
        readonly IDamageSystem damageSys;

        public ApplyDamage(ITaskIdBuilder id, IDamageSystem damageSys)
            : base(id)
        {
            this.damageSys = damageSys;
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
            state.unit = go.GetComponent<Unit>();
        }

        public override TaskResult Run(ITaskNode node)
        {
            Assert.IsFalse(string.IsNullOrEmpty(this.Profile));

            var state = (State)node.State;

            if (string.IsNullOrEmpty(this.Group))
            {
                this.damageSys.ApplyDamage(state.unit, this.Profile, state.unit);
            }
            else
            {
                var victims = state.groups.GetInGroup(this.Group);

                for (int i = 0; i < victims.Count; i++)
                {
                    if (ObjectValidator.Validate(victims[i]))
                    {
                        var victim = victims[i].GetComponent<Unit>();
                        this.damageSys.ApplyDamage(state.unit, this.Profile, victim);
                    }
                }

                victims.ReturnToPool();
            }

            return TaskResult.Success;
        }

        private class State : TaskState
        {
            public GameObjectGroup groups;
            public Unit unit;
        }
    }
}
