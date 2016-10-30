using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Andoco.Core.Pooling;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Movement.Movers;
using Andoco.Unity.Framework.Movement.Objectives;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialFieldNavigator : MonoBehaviour
    {
        private Transform tx;
        private Mover mover;
        private PotentialFilter filter;
        private IFieldNodeRef targetNode;
        private IFieldNodeRef nextNode;
        private PotentialFieldNodeSource targetNodeSource;
		private NextStrongestSampleStrategy potentialSampler;
        private bool isMoving;
        private bool isWorking;

        public bool navigateOnStart;
        public float speed = 1f;
        public PotentialLayerMask layerMask;
        public PotentialFieldSource source;
        public NodeSourceConf targetNodePotential;

        public string[] ignoreSourceKeys;

        public IPotentialFieldSystem PotentialField { get; set; }

        void Awake()
        {
            this.tx = this.transform;
            this.mover = this.GetComponent<Mover>();
			this.potentialSampler = new NextStrongestSampleStrategy();
        }

        void Start()
        {
            this.Spawned();
        }

        void Spawned()
        {
            if (this.PotentialField == null)
            {
                this.PotentialField = Indexed.GetSingle<IPotentialFieldSystem>();
            }

            if (this.navigateOnStart && this.PotentialField != null)
            {
                // TODO: Consider moving this into Update() to handle the transform position being set after spawning.
                this.StartNavigating();
            }
        }

        void Update()
        {
            if (this.PotentialField == null)
                return;
            
            if (!this.isMoving && !this.isWorking)
            {
                this.AddNextStrongest();
            }
        }

        void Recycled()
        {
            this.isMoving = false;
        }

        public void StartNavigating()
        {
            StartCoroutine(this.NavigationRoutine());
        }

        public void StopNavigating()
        {
        }

        #region Private methods

        private IEnumerator NavigationRoutine()
        {
            // Need to wait a frame in case the transform position is set after spawning.
            yield return null;

            while (this.mover.Driver == null)
            {
                yield return null;
            }

            this.mover.Driver.Speed = this.speed;
            this.mover.Driver.ObjectiveChanged += this.OnObjectiveChanged;

            this.AddNextStrongest();
        }

        private void OnObjectiveChanged(IMoveDriver driver, IObjective previous, IObjective current)
        {
            if (current == null)
            {
                this.AddNextStrongest();
            }
        }

        private void AddNextStrongest()
        {
            if (this.isWorking)
            {
                return;
            }

            this.isWorking = true;

            if (this.targetNode == null)
            {
                this.targetNode = this.PotentialField.GetClosestNode(this.tx.position);
            }

            // Disable the target node potential first so that it won't affect the choice of the next node. It will remain
            // disabled if no node with a higher potential is found.
            if (this.targetNodePotential.enabled)
            {
                if (this.targetNodeSource == null)
                {
                    this.targetNodeSource = this.source.AddNodeSource(this.targetNodePotential.sourceKey, this.targetNodePotential.layers.value, this.targetNodePotential.potential);
                    this.targetNodeSource.Calculator = this.targetNodePotential.calculator as IPotentialCalculator;
                }

                this.targetNodeSource.Enabled = false;
            }

            if (this.filter == null)
            {
                var go = this.gameObject;
                this.filter = (source) => (GameObject)source.Context != go || !ignoreSourceKeys.Contains(source.SourceKey);
            }

			var samples = this.PotentialField.SamplePotential(this.potentialSampler, this.targetNode, this.layerMask, this.filter);
			this.OnNextStrongestSearchFinished(samples);
			samples.ReturnToPool();
        }

        private void OnNextStrongestSearchFinished(IList<NodeSample> samples)
        {
            if (samples.Count == 0)
            {
                this.isMoving = false;
            }
            else
            {
                this.nextNode = samples[0].Node;
                this.mover.Driver.MoveTo(nextNode.Position);
                this.isMoving = true;

                if (this.targetNodePotential.enabled)
                {
                    this.targetNodeSource.Node = nextNode;
                    this.targetNodeSource.Enabled = true;
                }

                this.targetNode = nextNode;
                this.nextNode = null;
            }

            this.isWorking = false;
        }

        #endregion
    }
}
