namespace Andoco.Unity.Framework.Sensors
{
    using System;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public class DetectionCache : MonoBehaviour
    {
        private CacheItem[] cacheItems;
        private int numCached;

        private List<SensorSignalState> sensorSignalStates = new List<SensorSignalState>();

        [Inject]
        private IStructuredLog log;

        [Inject]
        private DetectionSignal detectionSignal;

        [Tooltip("The maximum number of detections that can be stored by the detection cache")]
        public int maxNumDetections = 10;

        public IList<GameObject> StartedDetections { get; private set; }

        public IList<GameObject> EndedDetections { get; private set; }

        void Awake()
        {
            this.cacheItems = new CacheItem[this.maxNumDetections];
            this.StartedDetections = new List<GameObject>();
            this.EndedDetections = new List<GameObject>();
        }

        void Update()
        {
            for (int i = 0; i < this.sensorSignalStates.Count; i++)
            {
                var state = this.sensorSignalStates[i];

                if (state.signalPending)
                {
                    this.detectionSignal.Dispatch(this.gameObject, new DetectionSignal.Data { Source = state.sensor.SensorName, Stage = state.stage });
                    state.signalPending = false;
                }
            }
        }

        void LateUpdate()
        {
            this.Evict();

            this.StartedDetections.RemoveInvalid();
            this.EndedDetections.RemoveInvalid();
        }

        void Recycled()
        {
            this.Cleanup(true, true);
            this.sensorSignalStates.Clear();

            for (int i = 0; i < this.cacheItems.Length; i++)
            {
                this.cacheItems[i].Reset();
            }
        }

        public void Cleanup(bool started, bool ended)
        {
            if (started)
                this.StartedDetections.Clear();

            if (ended)
                this.EndedDetections.Clear();
        }

        public void Detected(ISensor sensor, SensorStage stage, Collider collider, Collision collision)
        {
            switch (stage)
            {
                case SensorStage.Enter:
                    this.Put(sensor, collider);
                    break;
                case SensorStage.Exit:
                    this.Evict(collider);
                    break;
            }
        }

        public void Fill(IList<GameObject> list)
        {
            Assert.IsNotNull(list);

            if (this.numCached == 0)
                return;
            
            for (int i = 0; i < this.cacheItems.Length; i++)
            {
                // Select any valid values.
                if (this.cacheItems[i].Validate())
                {
                    list.Add(GetGameObject(this.cacheItems[i].value));
                }
            }
        }

        private static GameObject GetGameObject(object value)
        {
            if (value is Collider)
                return ((Collider)value).gameObject;
            else if (value is Collision)
                return ((Collision)value).gameObject;
            else
                throw new System.InvalidOperationException(string.Format("Encountered an unrecognised value type in the DetectionCache. Value = {0}", value));
        }

        private void Put(ISensor sensor, UnityEngine.Object value)
        {
            Assert.IsNotNull(value);

            if (this.log.Status.IsTraceEnabled)
                this.log.Trace("Adding object to detection cache", x => x.Field("value", value));

            int freeIndex = -1;

            if (this.numCached == 0)
            {
                freeIndex = 0;
            }
            else
            {
                for (int i = 0; i < this.cacheItems.Length; i++)
                {
                    // Exit early if aleady cached.
                    if (this.cacheItems[i].value == value && this.cacheItems[i].sensor == sensor)
                        return;

                    // Record the first free slot.
                    if (freeIndex == -1 && !this.cacheItems[i].hasValue)
                        freeIndex = i;
                }
            }

            if (freeIndex == -1)
            {
                this.log.Error("Failed to cache object because no free slots were available", x => x
                    .Field("value", value)
                    .Field("numCached", this.numCached)
                    .Field("numSlots", this.cacheItems.Length));
            }
            else
            {
                this.cacheItems[freeIndex].Set(sensor, value);

                this.numCached++;

                this.StartedDetections.Add(GetGameObject(value));
                this.SetSignalPending(sensor, SensorStage.Enter);

                if (this.log.Status.IsTraceEnabled)
                    this.log.Trace("Added object to detection cache", x => x
                        .Field("value", value)
                        .Field("numCached", this.numCached));
            }
        }

        private void Evict(UnityEngine.Object value = null)
        {
            if (this.numCached == 0)
                return;

            var items = this.cacheItems;
            
            for (int i = 0; i < items.Length; i++)
            {
                // Do nothing if the slot is already empty.
                if (!items[i].hasValue)
                    continue;

                if (items[i].Validate())
                {
                    // Evict specific value.
                    if (value != null && items[i].value == value)
                    {
                        this.EndedDetections.Add(GetGameObject(value));
                        this.SetSignalPending(items[i].sensor, SensorStage.Exit);
                        items[i].Reset();
                        this.numCached--;
                        break;
                    }
                }
                else
                {
                    // Evict any destroyed object values.
                    this.SetSignalPending(items[i].sensor, SensorStage.Exit);
                    items[i].Reset();
                    this.numCached--;
                }
            }
        }

        private void SetSignalPending(ISensor sensor, SensorStage stage)
        {
            for (int i = 0; i < this.sensorSignalStates.Count; i++)
            {
                if (string.Equals(sensor, this.sensorSignalStates[i].sensor))
                {
                    this.sensorSignalStates[i].signalPending = true;
                    this.sensorSignalStates[i].stage = stage;
                    return;
                }
            }

            var state = new SensorSignalState { sensor = sensor, signalPending = true, stage = stage };
            this.sensorSignalStates.Add(state);
        }

        #region Types

        private struct CacheItem
        {
            public bool hasValue;
            public UnityEngine.Object value;
            public ISensor sensor;

            public void Reset()
            {
                this.hasValue = false;
                this.value = null;
                this.sensor = null;
            }

            public void Set(ISensor sensor, UnityEngine.Object value)
            {
                this.sensor = sensor;
                this.value = value;
                this.hasValue = true;
            }

            public bool Validate()
            {
                return this.hasValue && ObjectValidator.Validate(this.value);
            }
        }

        private class SensorSignalState
        {
            public ISensor sensor;
            public bool signalPending;
            public SensorStage stage;
        }

        #endregion
    }
}
