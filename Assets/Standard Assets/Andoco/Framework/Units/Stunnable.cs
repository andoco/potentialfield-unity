using System.Collections;
using Andoco.BehaviorTree.Signals;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Units.Signals;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.Units
{
    public class Stunnable : MonoBehaviour
    {
        public const string StunnedFlag = "Stunned";
        public const string RecoveredFlag = "Recovered";

        [Inject]
        private FlagSignal flagSignal;

        public bool dispatchSignals = true;

        [Tooltip("A corresponding recovery will be required for each time the object was stunned")]
        public bool accumulateStuns = true;

        public bool IsStunned { get; private set; }

        /// <summary>
        /// A count of the number of stuns that must be recovered from.
        /// </summary>
        public int StunCounter { get; private set; }

        void Recycled()
        {
            this.IsStunned = false;
            this.StunCounter = 0;
        }

        public void Stun()
        {
            this.IsStunned = true;
            this.StunCounter++;

            if (this.dispatchSignals)
            {
                this.flagSignal.Dispatch(this.gameObject, new FlagSignal.Data(StunnedFlag));
            }
        }

        public void Recover()
        {
            if (!this.IsStunned)
                return;

            this.StunCounter--;

            if (this.accumulateStuns && this.StunCounter > 0)
            {
                return;
            }
            
            this.IsStunned = false;
            this.StunCounter = 0;

            if (this.dispatchSignals)
            {
                this.flagSignal.Dispatch(this.gameObject, new FlagSignal.Data(RecoveredFlag));
            }
        }
    }
}
