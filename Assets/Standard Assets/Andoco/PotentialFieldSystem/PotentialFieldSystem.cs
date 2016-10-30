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
        private ParticleSystem debugParticleSystem;
        private ParticleSystem.Particle[] debugParticles;
        private float highestPotential;
		private float time;
		private PotentialCache potentialCache;
		private PotentialFlowHandler flowHandler;

		public PotentialFieldData data;
        public SampleMode sampleMode;

        public float gizmoRadius = 1f;
        public Color neutral = Color.clear;
        public Color positive1 = new Color(1f, 0f, 0f);
        public Color positive2 = new Color(1f, 1f, 0f);
        public Color negative1 = new Color(0f, 0f, 1f);
        public Color negative2 = new Color(0f, 0.956862745f, 1f);

        public bool debugEnabled;
        public PotentialLayerMask debugLayerMask;
        public float debugPotentialScale = 1;
        public bool debugPotentialAutoScale = true;
        public Material debugParticleMaterial;
        public float debugParticleSize = 1;
        public ParticleSystemRenderMode debugParticleRenderMode;
        public DebugPositionMode debugPositionMode;
        public Vector3 debugPositionOffset;
        public float debugPositionScale = 1f;

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

            if (this.IsReady)
            {
                if (this.debugEnabled)
                {
                    if (this.debugParticleSystem == null)
                    {
                        this.BuildDebugParticleSystem();
                    }

                    var sampleRequest = new SampleRequest
                    {
                        nodes = this.nodes,
                        potentialLayerMask = this.debugLayerMask.value
                    };

                    var potentials = this.SamplePotential(sampleRequest);

                    if (this.debugPotentialAutoScale)
                    {
                        this.highestPotential = 0f;

                        for (int i = 0; i < this.nodes.Count; i++)
                        {
                            var potential = potentials[i];

                            this.highestPotential = Mathf.Max(this.highestPotential, Mathf.Abs(potential));
                        }

                        this.debugPotentialScale = this.highestPotential > 0f ? 1f / this.highestPotential : 1;
                    }

                    for (int i = 0; i < this.nodes.Count; i++)
                    {
                        var n = this.nodes[i];
                        var potential = potentials[i];
                        var normPotential = Mathf.Clamp(this.debugPotentialScale * potential, -1f, 1f);

                        debugParticles[i].startColor = ColorHelper.HeatMapColor(normPotential, this.neutral, this.positive1, this.positive2, this.negative1, this.negative2);
                    }

                    potentials.ReturnToPool();

                    debugParticleSystem.SetParticles(debugParticles, debugParticles.Length);
                }
                else
                {
                    if (this.debugParticleSystem != null)
                    {
                        Destroy(this.debugParticleSystem);
                        this.debugParticleSystem = null;
                        this.debugParticles = null;
                    }
                }
            }
        }

        //void OnDrawGizmos()
        //{
        //    if (this.IsReady)
        //    {
        //        for (int i = 0; i < this.nodes.Count; i++)
        //        {
        //            var n = this.nodes[i];
        //            var potential = this.GetPotential(n, this.debugLayerMask.value);
        //            var normPotential = 1f / this.debugPotentialScale * Mathf.Clamp(potential, -this.debugPotentialScale, this.debugPotentialScale);
        //            var radius = this.gizmoRadius;

        //            Gizmos.color = ColorHelper.HeatMapColor(normPotential, this.neutral, this.positive1, this.positive2, this.negative1, this.negative2);
        //            Gizmos.DrawSphere(n.Position, radius);
        //        }
        //    }
        //}

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

			var flowLayers = this.layers.Where(x => x.Kind == PotentialLayerKind.Flow).ToArray();

			this.flowHandler = new PotentialFlowHandler(graph, flowLayers);
			this.flowHandler.Decay = this.flowConfig.decay;
			this.flowHandler.Momentum = this.flowConfig.momentum;

            this.IsReady = true;

			StartCoroutine(this.UpdatePotentialFlow());
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

        public void ToggleDebug()
        {
            this.debugEnabled = !this.debugEnabled;
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
							if (layer.Kind == PotentialLayerKind.Flow)
							{
								potential = this.CombinePotential(potential, this.flowHandler.Sample(layer.LayerNum, node.Node));
								continue;
							}

							for (int k = 0; k < layer.Sources.Count; k++)
							{
								var source = layer.Sources[k];

								if (source.Enabled && (request.filter == null || request.filter(source)))
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

									potential = this.CombinePotential(potential, sourcePotential);
								}
							}
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

		public IList<NodeSample> SamplePotential(IPotentialSampleStrategy strategy, IFieldNodeRef startNode, int layerMask, PotentialFilter filter)
		{
			return strategy.SampleNodes(this, startNode, layerMask, filter);
		}

		public void ScheduleSamplePotential(IPotentialSampleStrategy strategy, IFieldNodeRef startNode, int layerMask, PotentialFilter filter, NodeSamplerDelegate callback)
		{
			throw new System.NotImplementedException();
		}

        #region Private methods

        private void BuildDebugParticleSystem()
        {
            debugParticleSystem = this.gameObject.AddComponent<ParticleSystem>();
            debugParticleSystem.maxParticles = int.MaxValue;
            debugParticleSystem.startSize = this.debugParticleSize;
            debugParticleSystem.startLifetime = float.MaxValue;

            var particleRenderer = debugParticleSystem.GetComponent<ParticleSystemRenderer>();
            particleRenderer.material = this.debugParticleMaterial;
            particleRenderer.renderMode = this.debugParticleRenderMode;

            var shape = debugParticleSystem.shape;
            shape.enabled = false;
            var emission = debugParticleSystem.emission;
            emission.enabled = false;

            debugParticles = new ParticleSystem.Particle[this.nodes.Count];

            for (int i = 0; i < this.nodes.Count; i++)
            {
                Vector3 pos;
                switch (this.debugPositionMode)
                {
                    case DebugPositionMode.None:
                        pos = this.nodes[i].Position;
                        break;
                    case DebugPositionMode.Offset:
                        pos = this.nodes[i].Position + debugPositionOffset;
                        break;
                    case DebugPositionMode.ScaleOut:
                        pos = this.nodes[i].Position * this.debugPositionScale;
                        break;
                    default:
                        throw new System.InvalidOperationException(string.Format("Unknown debug position mode {0}", this.debugPositionMode));
                }

                debugParticles[i] = new ParticleSystem.Particle();
                debugParticles[i].position = pos;
                debugParticles[i].startSize = this.debugParticleSize;
                debugParticles[i].startLifetime = float.MaxValue;
            }

            debugParticleSystem.SetParticles(debugParticles, debugParticles.Length);

            debugParticleSystem.Play();
        }

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

		private IEnumerator UpdatePotentialFlow()
		{
			while (true)
			{
				this.flowHandler.Propagate();

				yield return new WaitForSeconds(1f / this.flowConfig.updateFrequency);
			}
		}

        #endregion

        #region Types

        public class Layer
        {
            public Layer(int layerNum, PotentialLayerKind layerKind)
            {
                this.LayerNum = layerNum;
				this.Kind = layerKind;
                this.Sources = new List<PotentialFieldNodeSource>();
            }

            public int LayerNum { get; private set; }

            public int LayerMask { get { return 1 << this.LayerNum; } }

			public PotentialLayerKind Kind { get; private set; }

            public List<PotentialFieldNodeSource> Sources { get; private set; }

            public bool IsOneOfLayers(int layerMask)
            {
                return (this.LayerMask & layerMask) == this.LayerMask;
            }
        }

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

        public enum DebugPositionMode
        {
            None,
            Offset,
            ScaleOut
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
