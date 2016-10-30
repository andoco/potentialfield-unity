using System.Collections.Generic;

namespace Andoco.Unity.Framework.PotentialField
{
    public interface IPotentialSource
    {
        IList<PotentialFieldNodeSource> GetNodeSources();
    }
}
