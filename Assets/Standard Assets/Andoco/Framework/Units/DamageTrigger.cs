namespace Andoco.Unity.Framework.Units
{
    using UnityEngine;
    using Zenject;

    public class DamageTrigger : MonoBehaviour
    {
        Unit unit;

        [Inject]
        IDamageSystem damageSys;

        public bool onTrigger;
        public string[] damageProfiles;

        public void Trigger(Unit otherUnit)
        {
            if (unit == null)
            {
                unit = GetComponentInParent<Unit>();
            }

            foreach (var key in damageProfiles)
            {
                damageSys.ApplyDamage(unit, key, otherUnit);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            var otherUnit = other.GetComponentInParent<Unit>();
            Trigger(otherUnit);
        }
    }
}
