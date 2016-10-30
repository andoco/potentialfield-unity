namespace Andoco.Unity.Framework.Level.Tasks
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using UnityEngine;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Level.Objectives;

    public class ResetLevelObjectives : ActionTask
    {
        private readonly ILevelSystem level;

        public ResetLevelObjectives(ITaskIdBuilder id, ILevelSystem level)
            : base(id)
        {
            this.level = level;
        }

        public string Category { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            this.level.ResetLevelObjectives(this.Category);

            return TaskResult.Success;
        }
    }
}
