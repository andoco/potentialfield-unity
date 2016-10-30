using System.Collections.Generic;
using Andoco.Unity.Framework.Core;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public delegate bool PotentialFilter(PotentialFieldNodeSource source);

    public interface IPotentialFieldSystem
    {
        IFieldNodeRef this[int index] { get; }

        int NumNodes { get; }

        void SetGraph(SimpleGraph graph, Vector3[] positions);

        IList<IFieldNodeRef> GetNeighbours(IFieldNodeRef node);

        void ToggleDebug();

        void AddNodeSource(PotentialFieldNodeSource nodeSource);

        PotentialFieldNodeSource AddNodeSource(object context, string sourceKey, int layer, float potential = 0f);

        void RemoveNodeSource(PotentialFieldNodeSource nodeSource);

        IFieldNodeRef GetClosestNode(Vector3 position, IFieldNodeRef startNode = null);

        IList<float> SamplePotential(SampleRequest request);

		IList<NodeSample> SamplePotential(IPotentialSampleStrategy strategy, IFieldNodeRef startNode, int layerMask, PotentialFilter filter);

		void ScheduleSamplePotential(IPotentialSampleStrategy strategy, IFieldNodeRef startNode, int layerMask, PotentialFilter filter, NodeSamplerDelegate callback);
    }
}
