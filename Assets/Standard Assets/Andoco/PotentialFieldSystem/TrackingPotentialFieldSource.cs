using System;
using Andoco.Unity.Framework.Core;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class TrackingPotentialFieldSource : MonoBehaviour
    {
        private Transform cachedTx;
        private IPotentialFieldSystem system;
        private IFieldNodeRef[] nodeHistory;
        private PotentialFieldNodeSource[] nodeSources;
        private Vector3 previousPosition;
        private float nodeDistanceThresholdSquared;

        public PotentialFieldSource source;

        [Tooltip("The distance from the current node that the propagator can move before checking for the closest node.")]
        public float nodeDistanceThreshold = 0.1f;

        public SourceEntry[] sourceConfigs;

        void Awake()
        {
            this.cachedTx = this.transform;
            this.nodeDistanceThresholdSquared = this.nodeDistanceThreshold * this.nodeDistanceThreshold;

            if (this.source == null)
            {
                this.source = GetComponent<PotentialFieldSource>();
            }
        }

        void Start()
        {
            if (this.system == null)
            {
                this.system = Indexed.GetSingle<IPotentialFieldSystem>();
            }

            this.nodeSources = new PotentialFieldNodeSource[sourceConfigs.Length];

            var maxAge = 0;

            for (int i = 0; i < this.sourceConfigs.Length; i++)
            {
                var conf = sourceConfigs[i];

                this.nodeSources[i] = this.source.AddNodeSource(conf.sourceKey, conf.layers, conf.potential);
                this.nodeSources[i].Calculator = conf.calculator as IPotentialCalculator;
				this.nodeSources[i].Flow = conf.flow;
                this.nodeSources[i].Enabled = conf.enabled;

                maxAge = Mathf.Max(conf.age, maxAge);
            }

            this.nodeHistory = new IFieldNodeRef[maxAge + 1];

            this.Spawned();
        }

        void Spawned()
        {
            UpdateClosestNode();
        }

        void FixedUpdate()
        {
            if (this.nodeHistory[0] != null)
            {
                var sqrDistFromNode = (this.cachedTx.position - this.nodeHistory[0].Position).sqrMagnitude;

                if (sqrDistFromNode >= this.nodeDistanceThresholdSquared)
                {
                    this.UpdateClosestNode();
                }
            }

            // Disable any trailing node sources if we haven't moved since last update.
            if (this.cachedTx.transform.position == this.previousPosition)
            {
                for (int i = 0; i < this.sourceConfigs.Length; i++)
                {
                    var conf = this.sourceConfigs[i];

                    if (conf.enabled && conf.age > 0)
                    {
                        this.nodeSources[i].Enabled = false;
                    }
                }
            }

            this.previousPosition = this.cachedTx.position;
        }

        void Recycled()
        {
            Array.Clear(this.nodeHistory, 0, this.nodeHistory.Length);
        }

        private void UpdateClosestNode()
        {
            var currentClosestNode = this.nodeHistory[0];
            var closestNode = this.system.GetClosestNode(this.cachedTx.position, currentClosestNode);

            if (closestNode != currentClosestNode)
            {
                // Move nodes down in the history.
                for (int i = 0; i < this.nodeHistory.Length - 1; i++)
                {
                    this.nodeHistory[i + 1] = this.nodeHistory[i];
                }

                // Set new closest node in the history.
                this.nodeHistory[0] = closestNode;

                // Update the nodes of the potential sources.
                for (int i = 0; i < this.sourceConfigs.Length; i++)
                {
                    var conf = this.sourceConfigs[i];
                    var source = this.nodeSources[i];
                    source.Node = this.nodeHistory[conf.age];

                    // Automatically enable trailing nodes.
                    if (conf.age > 0 && this.nodeHistory[conf.age] != this.nodeHistory[conf.age - 1])
                    {
                        source.Enabled = true;
                    }
                }
            }
        }

        [System.Serializable]
        public class SourceEntry
        {
            public bool enabled;
            public int age;
            public PotentialLayerMask layers;
            public float potential;
            public Component calculator;
            public string sourceKey;
			public PotentialFlowKind flow;
        }
    }
}
