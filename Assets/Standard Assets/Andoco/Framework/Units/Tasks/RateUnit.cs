namespace Andoco.Unity.Framework.Units
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Level;

    public class RateUnit : ActionTask
    {
        public RateUnit(ITaskIdBuilder id)
            : base(id)
        {
        }

        public override TaskResult Run(ITaskNode node)
        {
            var levelRating = Indexed.GetSingle<LevelRating>();
            var unit = node.Context.GetGameObject().GetComponent<Unit>();

            unit.strength.Configure(1f, levelRating.CalculateLinear(unit.strengthRating));
            unit.strength.Value = unit.strength.Max;

            unit.health.Configure(0f, levelRating.CalculateLinear(unit.healthRating));
            unit.health.Value = unit.health.Max;

            return TaskResult.Success;
        }
    }
}
