namespace Andoco.Unity.Framework.Level.Tasks
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using UnityEngine;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Level.Objectives;
    
    public class CheckLevelObjectives : ActionTask
    {
        private readonly ILevelSystem level;

        public CheckLevelObjectives(ITaskIdBuilder id, ILevelSystem level)
            : base(id)
        {
            this.level = level;
        }

        public string Category { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            // TODO: Remove this as it isn't very clear it will happen.
            if (!this.level.IsStarted)
            {
                return TaskResult.Pending;
            }

            var result = this.level.CheckRequiredObjectives(this.Category);

            switch (result)
            {
                case LevelObjectiveResult.Completed:
                    return TaskResult.Success;
                case LevelObjectiveResult.Failed:
                    return TaskResult.Failure;
                case LevelObjectiveResult.Pending:
                    return TaskResult.Pending;
                default:
                    throw new InvalidOperationException(string.Format("Unsupported {0} value {1}", typeof(LevelObjectiveResult), result));
            }
        }
    }
}
