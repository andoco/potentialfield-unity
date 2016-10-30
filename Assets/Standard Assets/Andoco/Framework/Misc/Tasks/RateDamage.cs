namespace Andoco.Unity.Framework.Misc
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Level;
    using Andoco.Unity.Framework.Units;

    public class RateDamage : ActionTask
    {
        public RateDamage(ITaskIdBuilder id)
            : base(id)
        {
        }

        public override TaskResult Run(ITaskNode node)
        {
            var go = node.Context.GetGameObject();
            var unit = go.GetComponent<Unit>();
            var damage = go.GetComponent<Damage>();

            damage.profiles[0].targetDamage = unit.strength.Value;

            return TaskResult.Success;
        }
    }
}
