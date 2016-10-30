using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine;

namespace Andoco.Unity.Framework.Level.Scoring
{
    public class AddScorePoints : ActionTask
    {
        public AddScorePoints(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Key { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var scoring = node.Context.GetComponent<Scoring>();

            scoring.AddPoints(this.Key);

            return TaskResult.Success;
        }
    }
}