using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.BehaviorTree.Decorators;
using Andoco.BehaviorTree.Scheduler;
using Andoco.Core.Signals;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Sensors
{
    public class ObserveDetection : SignalDecorator
    {
        private readonly DetectionSignal signal;

        public ObserveDetection(ITaskIdBuilder id, ITaskScheduler scheduler, DetectionSignal signal)
            : base(id, scheduler)
        {
            this.signal = signal;
            this.Stage = SensorStage.Enter;
        }

        public string Source { get; set; }

        public SensorStage Stage { get; set; }

        protected override ISlotController Listen(ITaskNode node)
        {
            var go = node.Context.GetGameObject();

            return this.signal.AddListener(go, data =>
            {
                if ((string.IsNullOrEmpty(this.Source) || 
                    string.Equals(this.Source, data.Source, StringComparison.OrdinalIgnoreCase)) &&
                    data.Stage == this.Stage)
                {
                    this.OnSignalled(node);
                }
            });
        }
    }
}
