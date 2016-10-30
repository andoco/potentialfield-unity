namespace Andoco.Unity.Framework.Units.Tasks
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;

    public class CancelDamage : ActionTask
    {
        readonly IDamageSystem damageSys;

        public CancelDamage(ITaskIdBuilder id, IDamageSystem damageSys)
            : base(id)
        {
            this.damageSys = damageSys;
        }

        public string Profile { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var unit = node.Context.GetComponent<Unit>();

            this.damageSys.CancelDamage(unit, this.Profile);

            return TaskResult.Success;
        }
    }
}
