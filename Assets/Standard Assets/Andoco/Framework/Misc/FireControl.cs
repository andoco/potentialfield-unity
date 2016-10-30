namespace Andoco.Unity.Framework.Misc
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;

    public class FireControl : ActionTask
    {
        public FireControl(ITaskIdBuilder id)
            : base(id)
        {
        }

        public ActionKind Action { get; set; }

        public string Group { get; set; }

        protected override void Started(ITaskNode node)
        {
            var firer = node.Context.GetGameObject().GetComponent<FireController>();

            switch (this.Action)
            {
                case ActionKind.Ceasefire:
                    firer.Ceasefire();
                    break;
                case ActionKind.Fire:
                    firer.Fire(this.Group);
                    break;
                case ActionKind.Unlock:
                    firer.Unlock();
                    break;
                default:
                    throw new System.InvalidOperationException(string.Format("Unknown fire action kind {0}", this.Action));
            }
        }

        public override TaskResult Run(ITaskNode node)
        {
            var firer = node.Context.GetGameObject().GetComponent<FireController>();

            return firer.IsFiring
                ? TaskResult.Pending
                : TaskResult.Success;
        }

        public enum ActionKind
        {
            None,
            Ceasefire,
            Fire,
            Unlock
        }
    }
}
