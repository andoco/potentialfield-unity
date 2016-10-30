namespace Andoco.Unity.Framework.Sensors
{
    using System;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Misc;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public class TriggerSensor : MonoBehaviour, ISensor
    {
        private int startFrame;

        [Inject]
        private IStructuredLog log;

        [Inject]
        private GameObjectGroup globalGroup;

        [Inject]
        private SensorSignal signal;

        [SerializeField]
        [EnumFlags]
        public SensorStage stages;

        [Tooltip("The number of frames to wait after adding to scene before sensor events are raised")]
        public int skipFrames;

        [Tooltip("If set, a group will be maintained containing objects currently being sensed.")]
        public GameObjectGroup maintainGroup;

        [Tooltip("The name of the group to maintain sensed objects in.")]
        public string maintainGroupName = "Sensed";

        public bool dispatchSignals;

        [Tooltip("The context to use when dispatching signals. Defaults to the current GameObject")]
        public GameObject signalContext;

        public SensorEvent sensed;

        public GameObjectGroup whitelistGroup;
        public string whitelistGroupName;
        public string globalWhitelistGroupName;

        public string SensorName { get { return this.gameObject.name; } }

        void Start()
        {
            if (this.signalContext == null)
                this.signalContext = this.gameObject;
            
            this.startFrame = Time.frameCount;
        }

        void Spawned()
        {
            this.startFrame = Time.frameCount;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!this.Validate(other))
                return;
            
            if ((this.stages & SensorStage.Enter) == SensorStage.Enter)
            {
                if (this.log.Status.IsInfoEnabled)
                    this.log.Info("A collider has entered the sensor", x => x
                        .Field("stage", SensorStage.Enter)
                        .Field("other", other)
                        .Field("otherEntity", other.GetEntityInParent()));

                if (this.maintainGroup != null)
                {
                    this.maintainGroup.Add(other.gameObject, this.maintainGroupName);
                }

                if (this.dispatchSignals)
                    this.signal.Dispatch(this.signalContext, new SensorSignal.Data(this.SensorName, SensorStage.Enter, other));
                
                this.sensed.Invoke(this, SensorStage.Enter, other, null);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!this.Validate(other))
                return;

            if ((this.stages & SensorStage.Exit) == SensorStage.Exit)
            {
                if (this.log.Status.IsInfoEnabled)
                    this.log.Info("A collider has exited the sensor", x => x
                        .Field("stage", SensorStage.Exit)
                        .Field("other", other)
                        .Field("otherEntity", other.GetEntityInParent()));

                if (this.maintainGroup != null)
                {
                    this.maintainGroup.Remove(other.gameObject, this.maintainGroupName);
                }

                if (this.dispatchSignals)
                    this.signal.Dispatch(this.signalContext, new SensorSignal.Data(this.SensorName, SensorStage.Exit, other));

                this.sensed.Invoke(this, SensorStage.Exit, other, null);
            }
        }

        private bool Validate(Collider other)
        {
            if (other == null)
                return false;

            if (!this.CheckFrame())
                return false;

            if (!this.CheckWhitelist(other))
            {
                if (this.log.Status.IsTraceEnabled)
                    this.log.Trace("Failed whitelist check", x => x
                        .Field("other", other)
                        .Field("otherEntity", other.GetEntityInParent()));

                return false;
            }

            return true;
        }

        private bool CheckFrame()
        {
            if (Time.frameCount - this.startFrame > this.skipFrames)
                return true;

            return false;
        }

        private bool CheckWhitelist(Collider other)
        {
            var whitelistUsed = false;

            // Check local whitelist first.
            if (!string.IsNullOrEmpty(this.whitelistGroupName))
            {
                Assert.IsNotNull(this.whitelistGroup);

                whitelistUsed = true;

                if (this.whitelistGroup.IsInGroup(other.gameObject, this.whitelistGroupName) ||
                    this.whitelistGroup.IsInGroup(other.GetEntityGameObject(), this.whitelistGroupName))
                {
                    return true;
                }
            }

            // Check global whitelist.
            if (!string.IsNullOrEmpty(this.globalWhitelistGroupName))
            {
                whitelistUsed = true;

                if (this.globalGroup.IsInGroup(other.gameObject, this.globalWhitelistGroupName) ||
                    this.globalGroup.IsInGroup(other.GetEntityGameObject(), this.globalWhitelistGroupName))
                {
                    return true;
                }
            }

            // Both local and global whitelists failed.
            if (whitelistUsed)
                return false;
                
            // No whitelisting so return true.
            return true;
        }
    }
}
