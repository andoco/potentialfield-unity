namespace Andoco.Unity.Framework.Level.Tasks
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;

    public class CalculateLevelRating : ActionTask
    {
        public CalculateLevelRating(ITaskIdBuilder id)
            : base(id)
        {
        }

        public override TaskResult Run(ITaskNode node)
        {
            var levelRating = Indexed.GetSingle<LevelRating>();
            levelRating.Calculate();

            return TaskResult.Success;
        }
    }
}
