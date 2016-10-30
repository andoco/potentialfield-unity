namespace Andoco.Unity.Framework.Units.Tasks
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Conditions;
    using Andoco.Unity.Framework.BehaviorTree;

    public class CheckStunned : Condition
    {
        public CheckStunned(ITaskIdBuilder id)
            : base(id)
        {
        }

        protected override bool Evaluate(ITaskNode node)
        {
            var stunnable = node.Context.GetGameObject().GetComponent<Stunnable>();

            return stunnable.IsStunned;
        }
    }
}
