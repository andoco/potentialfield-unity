namespace Andoco.Unity.Framework.Level.Tasks
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Level.Schemas;
    using UnityEngine;

    public class BuildLevel : ActionTask
    {
        public BuildLevel(ITaskIdBuilder id)
            : base(id)
        {
        }

        public override TaskResult Run(ITaskNode node)
        {
            var levelSchemaSys = Indexed.GetSingle<ILevelSchemaSystem>();

            var schema = levelSchemaSys.Schemas[levelSchemaSys.Schemas.Count - 1];
            levelSchemaSys.SelectSchema(schema);
            levelSchemaSys.BuildLevel();

            return TaskResult.Success;
        }
    }
}
