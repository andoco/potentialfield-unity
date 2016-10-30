namespace Andoco.Unity.Framework.Units
{
    using UnityEngine;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Units.Signals;
    using Zenject;

    public class Unit : MonoBehaviour
    {
        private bool minStrengthSignalled;
        private bool minHealthSignalled;

        [Inject]
        private UnitStrengthSignal unitStrengthSignal;

        [Inject]
        private UnitHealthSignal unitHealthSignal;
    
        public ClampedFloat strength;
        public FloatRange strengthRating;
        public ClampedFloat health;
        public FloatRange healthRating;

        public DamageProfile[] damageProfiles;
    
        void Awake()
        {
        }

        void Spawned()
        {
        }

        void Update()
        {
            if (!this.minStrengthSignalled && this.strength.IsMin)
            {
                this.unitStrengthSignal.Dispatch(this.gameObject, new UnitStrengthSignal.Data(this, UnitStrengthState.MinReached));
                this.minStrengthSignalled = true;
            }

            if (!this.minHealthSignalled && this.health.IsMin)
            {
                this.unitHealthSignal.Dispatch(this.gameObject, new UnitHealthSignal.Data(this, UnitHealthState.MinReached));
                this.minHealthSignalled = true;
            }
        }
            
        void Recycled()
        {
            this.minHealthSignalled = false;
            this.minStrengthSignalled = false;
        }
    }
}
