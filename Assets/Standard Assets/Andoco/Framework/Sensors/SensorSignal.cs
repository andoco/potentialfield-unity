namespace Andoco.Unity.Framework.Sensors
{
    using System;
    using Andoco.Core.Signals;
    using UnityEngine;

    public class SensorSignal : Signal<SensorSignal.Data>
    {
        public struct Data
        {
            public Data(string name, SensorStage stage, Collider other)
                : this()
            {
                this.SensorName = name;
                this.Stage = stage;
                this.Other = other;
            }

            public string SensorName { get; private set; }
            public SensorStage Stage { get; private set; }
            public Collider Other { get; private set; }
        }
    }
}
