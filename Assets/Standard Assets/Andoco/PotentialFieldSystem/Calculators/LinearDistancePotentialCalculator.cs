using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    /// <summary>
    /// Returns a potential charge that is linearly inversely proportional to the distance of the sample position from the source.
    /// </summary>
    public class LinearDistancePotentialCalculator : MonoBehaviour, IPotentialCalculator
    {
        public float GetPotential(Vector3 samplePos, Vector3 sourcePos, float sourcePotential)
        {
            var d = Vector3.Distance(sourcePos, samplePos);
            var p = 1f / Mathf.Max(1f, d) * sourcePotential;

            return p;
        }
    }
}
