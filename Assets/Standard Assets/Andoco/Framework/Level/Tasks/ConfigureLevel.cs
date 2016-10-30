namespace Andoco.Unity.Framework.Level.Tasks
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;

    public class ConfigureLevel : ActionTask
    {
        public ConfigureLevel(ITaskIdBuilder id)
            : base(id)
        {
        }

        public override TaskResult Run(ITaskNode node)
        {
            var level = Indexed.GetSingle<ILevelSystem>();
            level.Configure(level.LevelNumber + 1);

            return TaskResult.Success;
        }
    }
}
