namespace Andoco.Unity.Framework.Sensors
{
    using System.Collections;
    using System.Collections.Generic;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;
    using UnityEngine.Events;

    public class ProximitySensor : MonoBehaviour, ISensor
    {
        private Transform cachedTransform;
//        private List<Collider> previousColliders = new List<Collider>();
        private Collider[] colliders;
        private Collider[] previousColliders;
        private int previousNumColliders;

        public float maxDistance;
        public LayerMask layers;
        public bool senseOnStart;
        public bool senseOnSpawned;

        [Tooltip("The interval in seconds between each activation of the sensor")]
        public float senseInterval = 0;

        public SensorEvent sensed;

        public string SensorName { get { return this.gameObject.name; } }

        void Awake()
        {
            this.cachedTransform = this.transform;
            this.colliders = new Collider[10];
        }

        void Start()
        {
            if (this.senseOnStart)
            {
                this.Sense();
            }

            StartCoroutine(this.SenseRoutine());
        }

        void Spawned()
        {
            if (this.senseOnSpawned)
            {
                this.Sense();
            }

            StartCoroutine(this.SenseRoutine());
        }

        public void Sense()
        {
//            var numSensed = Physics.OverlapSphereNonAlloc(this.cachedTransform.position, this.maxDistance, this.colliders, this.layers.value);
//
//            for (int i = 0; i < this.previousNumColliders; i++)
//            {
//                for (int j = 0; j < numSensed; j++)
//                {
//                    
//                }
//            }
//
//            for (int i = 0; i < numSensed; i++)
//            {
//                SensorStage stage;
//                var isNew = true;
//
//                // Check if this is a newly sensed collider.
//                for (int j = 0; j < this.previousNumColliders; j++)
//                {
//                    if (this.colliders[i] == this.previousColliders[j])
//                    {
//                        isNew = false;
//                        break;
//                    }
//                }
//
//                if (isNew)
//                {
//                    stage = SensorStage.Enter;
//                }
//
//
//            }

//            {
//
//
//                var index = this.previousColliders.IndexOf(c);
//                var isNew = index != -1;
//                SensorStage stage;
//
//                if (isNew)
//                {
//                    stage = SensorStage.Enter;
//                    this.previousColliders.Add(c);
//                }
//
//                this.sensed.Invoke(stage, c, null);
//            }
        }

        private IEnumerator SenseRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(this.senseInterval);

                this.Sense();
            }
        }
    }

}
