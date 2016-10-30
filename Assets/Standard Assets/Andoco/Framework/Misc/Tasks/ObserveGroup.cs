namespace Andoco.Unity.Framework.Misc
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Decorators;
    using Andoco.BehaviorTree.Scheduler;
    using Andoco.Core.Signals;
    using Andoco.Unity.Framework.BehaviorTree;

    public class ObserveGroup : SignalDecorator
    {
        private readonly GameObjectGroupSignal signal;

        public ObserveGroup(ITaskIdBuilder id, ITaskScheduler scheduler, GameObjectGroupSignal signal)
            : base(id, scheduler)
        {
            this.signal = signal;
        }

        public string Name { get; set; }

        public CheckKind Check { get; set; }

        protected override ISlotController Listen(ITaskNode node)
        {
            var go = node.Context.GetGameObject();

            return this.signal.AddListener(go, data =>
            {
                if (string.Equals(data.name, this.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var shouldSignal = false;

                    switch (this.Check)
                    {
                        case CheckKind.None:
                            break;
                        case CheckKind.Empty:
                            shouldSignal = data.group.IsEmpty(this.Name);
                            break;
                    }

                    if (shouldSignal)
                    {
                        this.OnSignalled(node);
                    }
                }
            });
        }

        public enum CheckKind
        {
            None,
            Empty
        }
    }
}

