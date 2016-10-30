using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public interface IPotentialCalculator
    {
        /// <summary>
        /// Calculates the potential charge at position <paramref name="pos"/> for a charge source at <paramref name="sourcePos"/>.
        /// </summary>
        /// <returns>The potential at the position <paramref name="pos"/>.</returns>
        /// <param name="samplePos">Position to sample the potential charge at.</param>
        /// <param name="sourcePos">Position of the potential charge source.</param>
        /// <param name="sourcePotential">The source potential charge present at the <paramref name="sourcePos"/>.
        float GetPotential(Vector3 samplePos, Vector3 sourcePos, float sourcePotential);
    }
}
