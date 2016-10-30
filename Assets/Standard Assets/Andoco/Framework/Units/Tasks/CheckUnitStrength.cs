namespace Andoco.Unity.Framework.Units.Tasks
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Conditions;
    using Andoco.Unity.Framework.BehaviorTree;
    
    public class CheckUnitStrength : Condition
    {
        public CheckUnitStrength(ITaskIdBuilder id)
            : base(id)
        {
        }
    
        public CheckKind Check { get; set; }
    
        protected override bool Evaluate(ITaskNode node)
        {
            var unit = node.Context.GetGameObject().GetComponent<Unit>();
    
            switch (this.Check)
            {
                case CheckKind.Any:
                    return unit.strength.Value > 0f;
                case CheckKind.Depleted:
                    return unit.strength.Value == 0f;
                case CheckKind.Full:
                    return unit.strength.IsMax;
                default:
                    throw new System.InvalidOperationException(string.Format("Unsupported {0} value {1}", typeof(CheckKind), this.Check));
            }
        }
    
        public enum CheckKind
        {
            None,
            Any,
            Depleted,
            Full
        }
    }
}
