namespace Andoco.Unity.Framework.Units
{
    using System;
    using System.Collections.Generic;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Core.Scene.Management;
    using UnityEngine;
    using Zenject;

    public class DamageSystem : MonoBehaviour, IDamageSystem
    {
        private readonly List<ScheduledEntry> scheduledDamage = new List<ScheduledEntry>();
        private readonly List<ScheduledEntry> remove = new List<ScheduledEntry>();

        [Inject]
        private IGameObjectManager goMgr;

        private Predicate<DamageProfile> findDamageProfilePredicate;

        public void ApplyDamage(Unit inflictor, string profileKey, Unit victim)
        {
            var profile = this.GetProfile(inflictor, profileKey);

            switch (profile.mode)
            {
                case DamageKind.DamageHealth:
                    this.DamageUnit(victim, profile.amount);
                    break;
                case DamageKind.DepleteHealth:
                    this.DamageUnit(victim, float.MaxValue);
                    break;
                case DamageKind.Destroy:
                    this.DestroyUnit(victim);
                    break;
                case DamageKind.Schedule:
                    this.ScheduleDamage(victim, profile.key, profile.frequency, profile.amount);
                    break;
                case DamageKind.None:
                    break;
            }
        }

        public void CancelDamage(Unit victim, string profileKey)
        {
            this.UnscheduleDamage(victim, profileKey);
        }

        private DamageProfile GetProfile(Unit unit, string profileKey)
        {
            if (this.findDamageProfilePredicate == null)
            {
                this.findDamageProfilePredicate = x => string.Equals(profileKey, x.key, StringComparison.OrdinalIgnoreCase);
            }

            return Array.Find(unit.damageProfiles, x => string.Equals(profileKey, x.key, StringComparison.OrdinalIgnoreCase));
        }

        private void DamageUnit(Unit unit, float damage)
        {
            if (damage == 0f)
                return;

            unit.health.Value -= damage;
        }

        private void DestroyUnit(Unit unit)
        {
            this.goMgr.Destroy(unit.gameObject);
        }

        public bool ScheduleDamage(Unit victim, string key, float frequency, float amount)
        {
            var entry = new ScheduledEntry(victim, key, frequency, amount);

            if (this.scheduledDamage.Contains(entry))
                return false;

            this.scheduledDamage.Add(entry);

            return true;
        }

        public void UnscheduleDamage(Unit unit, string profileKey)
        {
            var entry = new ScheduledEntry(unit, profileKey, 0f, 0f);

            this.scheduledDamage.Remove(entry);
        }

        void Update()
        {
            for (int i = 0; i < this.scheduledDamage.Count; i++)
            {
                var entry = this.scheduledDamage[i];

                if (!ObjectValidator.Validate(entry.victim))
                {
                    this.remove.Add(entry);
                    continue;
                }

                if (Time.time - entry.time < entry.frequency)
                {
                    continue;
                }

                entry.victim.health.Value -= entry.amount;

                if (entry.victim.health.IsMin)
                {
                    this.remove.Add(entry);
                }
                else
                {
                    this.scheduledDamage[i] = new ScheduledEntry(entry, Time.time);
                }
            }

            for (int i = 0; i < this.remove.Count; i++)
            {
                this.scheduledDamage.Remove(this.remove[i]);
            }

            this.remove.Clear();
        }

        private struct ScheduledEntry
        {
            public readonly Unit victim;
            public readonly string profileKey;
            public readonly float frequency;
            public readonly float amount;
            public readonly float time;

            public ScheduledEntry(Unit victim, string profileKey, float frequency, float amount)
            {
                this.victim = victim;
                this.profileKey = profileKey;
                this.frequency = frequency;
                this.amount = amount;
                this.time = 0f;
            }

            public ScheduledEntry(ScheduledEntry entry, float time)
            {
                this.victim = entry.victim;
                this.profileKey = entry.profileKey;
                this.frequency = entry.frequency;
                this.amount = entry.amount;
                this.time = time;
            }

            public override bool Equals(object obj)
            {
                var other = (ScheduledEntry)obj;

                return victim == other.victim && profileKey.Equals(other.profileKey, StringComparison.OrdinalIgnoreCase);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + victim.GetHashCode();
                    hash = hash * 23 + profileKey.GetHashCode();
                    return hash;
                }
            }
        }
    }
}
