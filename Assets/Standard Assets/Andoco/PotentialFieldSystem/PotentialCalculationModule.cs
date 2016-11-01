using System;
using System.Collections.Generic;

namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialCalculationModule
    {
        public float SamplePotential(SimpleGraphFieldNodeRef node, IList<PotentialFieldNodeSource> sources, PotentialFilter filter, Func<float, float, float> combiner)
        {
            var potential = 0f;

            for (int k = 0; k < sources.Count; k++)
            {
                var source = sources[k];

                if (source.Enabled && (filter == null || filter(source)))
                {
                    var sourcePotential = 0f;

                    if (node == source.Node)
                    {
                        sourcePotential = source.Potential;
                    }
                    else if (source.Calculator != null)
                    {
                        sourcePotential = source.Calculator.GetPotential(node.Position, source.Node.Position, source.Potential);
                    }

                    potential = combiner(potential, sourcePotential);
                }
            }

            return potential;
        }
    }
}
