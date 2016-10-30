using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class AnimationCurvePotentialCalculator : MonoBehaviour, IPotentialCalculator
    {
        public AnimationCurve potentialCurve;
        public float maxDistance;
        public bool invert = true;

        public float GetPotential(Vector3 samplePos, Vector3 sourcePos, float sourcePotential)
        {
            var d = Mathf.Min(Vector3.Distance(sourcePos, samplePos), maxDistance);
            var t = 1f / this.maxDistance * d;
            var p = this.potentialCurve.Evaluate(t);

            if (invert)
                p = 1f - p;

            p *= sourcePotential;

            return p * Mathf.Sign(sourcePotential);
        }
    }
}
