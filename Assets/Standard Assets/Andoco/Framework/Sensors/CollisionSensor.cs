namespace Andoco.Unity.Framework.Sensors
{
    using System;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;

    public class CollisionSensor : MonoBehaviour, ISensor
    {
        [SerializeField]
        [EnumFlags]
        public SensorStage stages;

        public SensorEvent sensed;

        public string SensorName { get { return this.gameObject.name; } }

        void OnCollisionEnter(Collision collision)
        {
            if ((this.stages & SensorStage.Enter) == SensorStage.Enter)
            {
                this.sensed.Invoke(this, SensorStage.Enter, collision.collider, collision);
            }
        }
    }
}
