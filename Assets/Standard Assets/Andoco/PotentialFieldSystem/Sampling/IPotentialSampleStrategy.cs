using System.Collections.Generic;

namespace Andoco.Unity.Framework.PotentialField
{
	public delegate void NodeSamplerDelegate(IList<NodeSample> samples);

	public interface IPotentialSampleStrategy
	{
		IList<NodeSample> SampleNodes(IPotentialFieldSystem field, IFieldNodeRef startNode, int layerMask, PotentialFilter filter);
	}
}
