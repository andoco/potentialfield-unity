namespace Andoco.Unity.Framework.Units.Tasks
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Decorators;
    using Andoco.BehaviorTree.Scheduler;
    using Andoco.Core.Signals;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Units.Signals;

    public class ObserveUnitStrength : SignalDecorator
    {
        private readonly UnitStrengthSignal signal;

        public ObserveUnitStrength(ITaskIdBuilder id, ITaskScheduler scheduler, UnitStrengthSignal signal)
            : base(id, scheduler)
        {
            this.signal = signal;
        }

        public UnitStrengthState State { get; set; }

        protected override ISlotController Listen(ITaskNode node)
        {
            var go = node.Context.GetGameObject();

            return this.signal.AddListener(go, data => {
                if (data.State == this.State)
                {
                    this.OnSignalled(node);
                }
            });
        }
    }
}
