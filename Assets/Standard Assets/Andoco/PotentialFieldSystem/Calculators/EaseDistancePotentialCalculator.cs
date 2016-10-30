using UnityEngine;
using Andoco.Unity.Framework.Core;

namespace Andoco.Unity.Framework.PotentialField
{
    public class EaseDistancePotentialCalculator : MonoBehaviour, IPotentialCalculator
    {
        Interpolate.Function easeFunc;

        public Interpolate.EaseType easeType;
        public float maxDistance;

        public float GetPotential(Vector3 samplePos, Vector3 sourcePos, float sourcePotential)
        {
            if (sourcePotential == 0f)
                return 0f;

            if (maxDistance == 0f)
                return sourcePotential;

            if (easeFunc == null)
                easeFunc = Interpolate.Ease(this.easeType);

            // start, distance, elapsed, duration.
            var potential = easeFunc(
                0f, 
                Mathf.Abs(sourcePotential), 
                maxDistance - Mathf.Min(Vector3.Distance(sourcePos, samplePos), maxDistance), 
                maxDistance);

            return potential * Mathf.Sign(sourcePotential);
        }
    }
}
