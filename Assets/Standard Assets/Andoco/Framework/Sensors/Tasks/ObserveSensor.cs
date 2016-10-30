using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.BehaviorTree.Decorators;
using Andoco.BehaviorTree.Scheduler;
using Andoco.Core.Signals;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine;

namespace Andoco.Unity.Framework.Sensors
{

    public class ObserveSensor : SignalDecorator
    {
        private readonly SensorSignal signal;

        public ObserveSensor(ITaskIdBuilder id, ITaskScheduler scheduler, SensorSignal signal)
            : base(id, scheduler)
        {
            this.signal = signal;
            this.Stage = SensorStage.Enter;
        }

        public string Sensor { get; set; }

        public SensorStage Stage { get; set; }

        protected override ISlotController Listen(ITaskNode node)
        {
            var go = node.Context.GetGameObject();

            return this.signal.AddListener(go, data =>
            {
                if ((string.IsNullOrEmpty(this.Sensor) || 
                    string.Equals(this.Sensor, data.SensorName, StringComparison.OrdinalIgnoreCase)) &&
                    data.Stage == this.Stage)
                {
                    this.OnSignalled(node);
                }
            });
        }
    }
}
