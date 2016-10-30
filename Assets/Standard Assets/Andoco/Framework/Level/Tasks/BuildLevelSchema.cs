namespace Andoco.Unity.Framework.Level.Tasks
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Level.Schemas;
    using UnityEngine;

    public class BuildLevelSchema : ActionTask
    {
        public BuildLevelSchema(ITaskIdBuilder id)
            : base(id)
        {
        }

        public override TaskResult Run(ITaskNode node)
        {
            var levelSchemaSys = Indexed.GetSingle<ILevelSchemaSystem>();

            var builder = levelSchemaSys.Builders[Random.Range(0, levelSchemaSys.Builders.Length)];
            var schema = builder.BuildSchema(0f);
            levelSchemaSys.AddSchema(schema);

            return TaskResult.Success;
        }
    }
    
}
