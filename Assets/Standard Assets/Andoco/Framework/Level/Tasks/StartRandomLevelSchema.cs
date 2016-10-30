namespace Andoco.Unity.Framework.Level.Tasks
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Level.Schemas;
    using UnityEngine;

    public class StartRandomLevelSchema : ActionTask
    {
        public StartRandomLevelSchema(ITaskIdBuilder id)
            : base(id)
        {
        }

        public override TaskResult Run(ITaskNode node)
        {
            var levelSchemaSys = Indexed.GetSingle<ILevelSchemaSystem>();

            if (levelSchemaSys.Schemas.Count == 0)
                levelSchemaSys.BuildRandomSchema();

            var schema = levelSchemaSys.Schemas[Random.Range(0, levelSchemaSys.Schemas.Count)];
            levelSchemaSys.SelectSchema(schema);
            levelSchemaSys.BuildLevel();

            return TaskResult.Success;
        }
    }
}
