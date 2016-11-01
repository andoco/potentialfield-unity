using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Andoco.Core.Pooling;
using Andoco.Unity.Framework.Core;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialFieldSystem : MonoBehaviour, IPotentialFieldSystem
    {
        private SimpleGraph graph;
        private List<IFieldNodeRef> nodes = new List<IFieldNodeRef>();
        private readonly List<Layer> layers = new List<Layer>();
		private float time;
		private PotentialCache potentialCache;
        private PotentialCalculationModule calculationModule;
        private PotentialFlowModule flowModule;

		public PotentialFieldData data;
        public SampleMode sampleMode;

		public PotentialFlowConfig flowConfig;

		public CacheConfig cacheConfig;

        public IFieldNodeRef this[int index] { get { return this.nodes[index]; } }

        public int NumNodes { get { return this.nodes.Count; } }

        public bool IsReady { get; private set; }

		void Awake()
		{
			if (this.cacheConfig.enabled)
			{
				this.potentialCache = new PotentialCache(this.cacheConfig.lifetime);
			}
		}

        void Update()
        {
			this.time = Time.time;
        }

        public void SetGraph(SimpleGraph graph, Vector3[] positions)
        {
            Debug.LogFormat("Setting graph with {0} nodes", graph.NumNodes);
            this.graph = graph;
            this.nodes.Clear();

            for (int i = 0; i < graph.NumNodes; i++)
            {
                this.nodes.Add(new SimpleGraphFieldNodeRef(positions[i], i));
            }

            Debug.LogFormat("Created {0} potential field nodes", this.nodes.Count);

            foreach (var layerConf in this.data.layers)
            {
                var layerKind = layerConf.flowField ? PotentialLayerKind.Flow : PotentialLayerKind.Field;
                var layer = new Layer(layerConf.value, layerKind);
                this.layers.Add(layer);
            }

            this.calculationModule = new PotentialCalculationModule();

			var flowLayers = this.layers.Where(x => x.Kind == PotentialLayerKind.Flow).ToArray();

			this.flowModule = new PotentialFlowModule(graph, flowLayers);
			this.flowModule.Decay = this.flowConfig.decay;
			this.flowModule.Momentum = this.flowConfig.momentum;

            this.IsReady = true;

			StartCoroutine(this.UpdatePotentialFlow());

            var debug = GetComponent<PotentialFieldDebugModule>();
            if (debug != null)
            {
                debug.Init(this);
            }
        }

        public IList<IFieldNodeRef> GetNeighbours(IFieldNodeRef node)
        {
            var graphNode = (SimpleGraphFieldNodeRef)node;
            var numArcs = this.graph.NumArcsForNode(graphNode.Node);
            var results = ListPool<IFieldNodeRef>.Take();

            for (int i = 0; i < numArcs; i++)
            {
                var neighbour = this.graph.GetNodeArc(graphNode.Node, i);
                results.Add(this.nodes[neighbour]);
            }

            return results;
        }

        public void AddNodeSource(PotentialFieldNodeSource nodeSource)
        {
            for (int i = 0; i < this.layers.Count; i++)
            {
                var layer = this.layers[i];

                if (layer.IsOneOfLayers(nodeSource.Layers))
                {
                    layer.Sources.Add(nodeSource);
                }
            }
        }

        public PotentialFieldNodeSource AddNodeSource(object context, string sourceKey, int layers, float potential = 0f)
        {
            var source = new PotentialFieldNodeSource(context, sourceKey, layers)
            { 
                Potential = potential,
                Enabled = true
            };

            this.AddNodeSource(source);

            return source;
        }

        public void RemoveNodeSource(PotentialFieldNodeSource nodeSource)
        {
            for (int i = 0; i < this.layers.Count; i++)
            {
                var layer = this.layers[i];

                if (layer.IsOneOfLayers(nodeSource.Layers))
                {
                    layer.Sources.Remove(nodeSource);
                }
            }
        }

        public IFieldNodeRef GetClosestNode(Vector3 position, IFieldNodeRef startNode = null)
        {
            int index;

            if (startNode == null)
            {
                index = this.GetClosestNodeIndex(position);
            }
            else
            {
                index = this.GetNodeIndexOfClosestNeighbourOrSelf(position, ((SimpleGraphFieldNodeRef)startNode).Node);
            }

            return this.nodes[index];
        }

        public IList<float> SamplePotential(SampleRequest request)
        {
            var potentials = ListPool<float>.Take();

			for (int i = 0; i < request.nodes.Count; i++)
            {
                var node = (SimpleGraphFieldNodeRef)request.nodes[i];
                var potential = 0f;

				var cacheHit = this.cacheConfig.enabled 
					&& this.potentialCache.TryGet(node.Node, request.potentialLayerMask, request.filter, this.time, out potential);

				if (!cacheHit)
				{
					for (int j = 0; j < this.layers.Count; j++)
					{
						var layer = this.layers[j];

                        if (layer.IsOneOfLayers(request.potentialLayerMask))
						{
                            var layerPotential = 0f;

                            switch (layer.Kind)
                            {
                                case PotentialLayerKind.Field:
                                    layerPotential = this.calculationModule.SamplePotential(node, layer.Sources, request.filter, this.CombinePotential);
                                    break;
                                case PotentialLayerKind.Flow:
                                    layerPotential = this.flowModule.Sample(layer.LayerNum, node.Node);
                                    break;
                                default:
                                    throw new System.InvalidOperationException(string.Format("Unknown potential layer kind {0}", layer.Kind));
                            }

                            potential = this.CombinePotential(potential, layerPotential);
						}
					}

					if (this.cacheConfig.enabled)
					{
						this.potentialCache.Set(node.Node, request.potentialLayerMask, request.filter, this.time, potential);
					}
				}

                potentials.Add(potential);
            }

            return potentials;
        }

		public IList<NodeSample> SamplePotential(IPotentialSampleStrategy strategy, IFieldNodeRef startNode, int layerMask, PotentialFilter filter)
		{
			return strategy.SampleNodes(this, startNode, layerMask, filter);
		}

		public void ScheduleSamplePotential(IPotentialSampleStrategy strategy, IFieldNodeRef startNode, int layerMask, PotentialFilter filter, NodeSamplerDelegate callback)
		{
			throw new System.NotImplementedException();
		}

        #region Private methods

        private int GetClosestNodeIndex(Vector3 position)
        {
            var minD = float.MaxValue;
            int minN = -1;

            for (int i = 0; i < this.nodes.Count; i++)
            {
                var n = this.nodes[i];
                var d = (position - n.Position).sqrMagnitude;

                if (d < minD)
                {
                    minN = i;
                    minD = d;
                }
            }

            return minN;
        }

        private int GetNodeIndexOfClosestNeighbourOrSelf(Vector3 pos, int node)
        {
            int closest = node;
            float closestDist = Vector3.SqrMagnitude(pos - this.nodes[node].Position);

            var numNeighbours = this.graph.NumArcsForNode(node);

            for (int i = 0; i < numNeighbours; i++)
            {
                var n = this.graph.GetNodeArc(node, i);
                var d = Vector3.SqrMagnitude(pos - this.nodes[n].Position);

                if (d < closestDist)
                {
                    closest = n;
                    closestDist = d;
                }
            }

            return closest;
        }

        private float CombinePotential(float potential, float sourcePotential)
        {
            switch (this.sampleMode)
            {
                case SampleMode.Additive:
                    potential += sourcePotential;
                    break;
                case SampleMode.Magnitude:
                    if (Mathf.Abs(sourcePotential) > Mathf.Abs(potential))
                    {
                        potential = sourcePotential;
                    }
                    break;
            }

            return potential;
        }

		private IEnumerator UpdatePotentialFlow()
		{
			while (true)
			{
				this.flowModule.Propagate();

				yield return new WaitForSeconds(1f / this.flowConfig.updateFrequency);
			}
		}

        #endregion

        #region Types

        public enum SampleMode
        {
            /// <summary>
            /// The potential from each source will be added together to obtain a final value.
            /// </summary>
            Additive,

            /// <summary>
            /// The potential with the greatest positive or negative magnitude will be the final value.
            /// </summary>
            Magnitude
        }

		[System.Serializable]
		public sealed class PotentialFlowConfig
		{
			[Tooltip("The percentage of potential (0..1) that will be lost during transfer from a neighbouring node")]
			public float decay = 0.3f;

			[Tooltip("The percentage of potential (0..1) that will be transferred from a neighbour node in a single update")]
			public float momentum = 0.8f;

			[Tooltip("The number of times per second the field will be updated")]
			public float updateFrequency = 3f;
		}

		[System.Serializable]
		public sealed class CacheConfig
		{
			public bool enabled;
			public float lifetime = 0.1f;
		}

        #endregion
    }
}
