using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    /// <summary>
    /// Always return a potential charge of zero.
    /// </summary>
    public class NullPotentialCalculator : MonoBehaviour, IPotentialCalculator
    {
        public float GetPotential(Vector3 samplePos, Vector3 sourcePos, float sourcePotential)
        {
            return 0f;
        }
    }
}
