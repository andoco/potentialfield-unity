using System.Collections.Generic;
using Andoco.Core.Pooling;

namespace Andoco.Unity.Framework.PotentialField
{
	public class NextStrongestSampleStrategy : IPotentialSampleStrategy
	{
		public IList<NodeSample> SampleNodes(IPotentialFieldSystem potentialField, IFieldNodeRef startNode, int layerMask, PotentialFilter filter)
		{
			var neighbours = potentialField.GetNeighbours(startNode);
			neighbours.Insert(0, startNode);
			var sampleRequest = new SampleRequest();

			sampleRequest.potentialLayerMask = layerMask;
			sampleRequest.filter = filter;
			sampleRequest.nodes = neighbours;

			var potentials = potentialField.SamplePotential(sampleRequest);

			var maxP = potentials[0];
			var maxN = 0;

			for (int i = 1; i < potentials.Count; i++)
			{
				var p2 = potentials[i];

				if (p2 > maxP)
				{
					maxN = i;
					maxP = p2;
				}
			}

			potentials.ReturnToPool();

			var results = ListPool<NodeSample>.Take();
			results.Add(new NodeSample(neighbours[maxN], maxP));

			neighbours.ReturnToPool();

			return results;
		}
	}
}
