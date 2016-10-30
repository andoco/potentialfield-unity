namespace Andoco.Unity.Framework.Sensors
{
    using Andoco.Core.Signals;
    using UnityEngine;

    public class DetectionSignal : Signal<DetectionSignal.Data>
    {
        public struct Data
        {
            public string Source { get; set; }

            public SensorStage Stage { get; set; }
        }
    }
}
