using System.Collections.Generic;

namespace Andoco.Unity.Framework.PotentialField
{
    public struct SampleRequest
    {
        public IList<IFieldNodeRef> nodes;
        public IList<float> potentials;
        public int potentialLayerMask;
        public PotentialFilter filter;
    }
}
