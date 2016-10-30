namespace Andoco.Unity.Framework.Sensors
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    [Serializable]
    public class SensorEvent : UnityEvent<ISensor, SensorStage, Collider, Collision>
    {
    }
}
