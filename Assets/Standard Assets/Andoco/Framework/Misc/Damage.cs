namespace Andoco.Unity.Framework.Misc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Core.Scene.Management;
    using Andoco.Unity.Framework.Units;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public class Damage : MonoBehaviour
    {
        private const string LogTargetKey = "target";
        private const string LogTargetUnitKey = "targetUnit";
        private const string LogTargetDamageAppliedKey = "targetDamageApplied";
        private const string LogTargetHealthBeforeKey = "targetHealthBefore";

        private readonly Dictionary<int, Coroutine> scheduledDamageRoutines = new Dictionary<int, Coroutine>();

        [Inject]
        private IStructuredLog log;

        [Inject]
        private IGameObjectManager goMgr;

        private Entity ownEntity;
        private int startFrame;

        public bool onTriggerEnter;

        public string onTriggerProfile;

        public int skipFrames;

        [Tooltip("If there is no unit associated with the collider, do not attempt to apply damage")]
        public bool skipNonUnitDamage;

        public Profile[] profiles;

        #region Lifecycle

        void Start()
        {
            this.ownEntity = this.GetEntityInParent();
            this.startFrame = Time.frameCount;
        }

        void Spawned()
        {
            this.startFrame = Time.frameCount;
        }

        void OnTriggerEnter(Collider other)
        {
            if (this.onTriggerEnter && this.CheckFrame())
            {
                this.DamageTarget(other, this.onTriggerProfile);
            }
        }

        void Recycled()
        {
            this.scheduledDamageRoutines.Clear();
        }

        #endregion

        #region Public methods

        public void DamageTarget(GameObject go, string profileKey)
        {
            this.DamageTarget(go.GetEntityInParent(), profileKey);
        }

        public void DamageTarget(Collider collider, string profileKey)
        {
            this.DamageTarget(collider.GetEntityInParent(), profileKey);
        }

        public void DamageTarget(Entity entity, string profileKey)
        {
            var profile = this.GetProfile(profileKey);

            if (profile.targetMode == DamageKind.Destroy)
            {
                this.DestroyEntity(entity);
            }
            else if (profile.targetMode == DamageKind.DamageHealth)
            {
                this.DamageUnitFor(entity, profile.targetDamage);
            }
            else if (profile.targetMode == DamageKind.DepleteHealth)
            {
                this.DamageUnitFor(entity, float.MaxValue);
            }

            if (profile.selfMode == DamageKind.Destroy)
            {
                this.DestroyEntity(this.ownEntity);
            }
            else if (profile.selfMode == DamageKind.DamageHealth)
            {
                this.DamageUnitFor(this.ownEntity, profile.selfDamage);
            }
            else if (profile.selfMode == DamageKind.DepleteHealth)
            {
                this.DamageUnitFor(this.ownEntity, float.MaxValue);
            }
        }

        public bool ScheduleDamage(int key, float frequency, float amount)
        {
            Coroutine routine;

            // Don't allow multiple damage routines with the same key.
            if (this.scheduledDamageRoutines.TryGetValue(key, out routine))
            {
                return false;
            }
            
            routine = this.StartCoroutine(this.ScheduledDamageRoutine(key, frequency, amount));
            this.scheduledDamageRoutines.Add(key, routine);

            return true;
        }

        public void UnscheduleDamage(int key)
        {
            Coroutine routine;

            if (this.scheduledDamageRoutines.TryGetValue(key, out routine))
            {
                this.StopCoroutine(routine);
                this.scheduledDamageRoutines.Remove(key);
            }
        }

        #endregion

        #region Private methods

        private bool CheckFrame()
        {
            if (Time.frameCount - this.startFrame > this.skipFrames)
                return true;

            return false;
        }

        private Profile GetProfile(string key)
        {
            for (int i = 0; i < this.profiles.Length; i++)
            {
                if (string.Equals(key, this.profiles[i].key, StringComparison.OrdinalIgnoreCase))
                {
                    return this.profiles[i];
                }
            }

            throw new InvalidOperationException(string.Format("Could not find a damage profile with the key {0}", key));
        }

        private void DamageUnitFor(Entity target, float damage)
        {
            if (damage == 0f)
                return;
            
            var unit = target.GetComponent<Unit>();

            if (unit == null)
            {
                if (this.skipNonUnitDamage)
                {
                    return;
                }

                throw new InvalidOperationException(string.Format("{0} must have a Unit within its entity. If unit damage was unintended, consider using a different collider.", target));
            }

            if (this.log.Status.IsInfoEnabled)
            {
                var msg = target == this ? "Apply damage to self" : "Applying damage to target";

                this.log.Info(msg, x => x
                    .Field(LogTargetKey, target)
                    .Field(LogTargetHealthBeforeKey, unit.health.Value)
                    .Field(LogTargetDamageAppliedKey, damage));
            }

            unit.health.Value -= damage;

            if (this.log.Status.IsInfoEnabled && unit.health.Value == 0f)
            {
                this.log.Info("Damage has depleted unit health", x => x
                    .Field(LogTargetKey, target)
                    .Field(LogTargetUnitKey, unit));
            }
        }

        private void DestroyEntity(Entity entity)
        {
            Assert.IsNotNull(entity, string.Format("{0} must have an enclosing entity", entity));

            if (this.log.Status.IsInfoEnabled && entity == this.ownEntity)
            {
                this.log.Info("Destroying own entity", x => x.Field(LogTargetKey, entity));
            }
            else
            {
                this.log.Info("Destroying target entity", x => x.Field(LogTargetKey, entity));
            }

            this.goMgr.Destroy(entity.gameObject);
        }

        private IEnumerator ScheduledDamageRoutine(int key, float frequency, float amount)
        {
            var unit = this.GetComponent<Unit>();

            while (true)
            {
                yield return new WaitForSeconds(frequency);

                unit.health.Value -= amount;

                if (unit.health.IsMin)
                    break;
            }

            this.scheduledDamageRoutines.Remove(key);
        }

        #endregion

        #region Types

        public enum DamageKind
        {
            None,
            DamageHealth,
            DepleteHealth,
            Destroy
        }

        [Serializable]
        public struct Profile
        {
            public string key;
            public DamageKind targetMode;
            public float targetDamage;
            public DamageKind selfMode;
            public float selfDamage;
        }

        #endregion
    }
}
