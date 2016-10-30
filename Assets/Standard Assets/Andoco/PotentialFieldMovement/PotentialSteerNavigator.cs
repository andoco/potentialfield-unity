using Andoco.Core.Diagnostics.Logging;
using Andoco.Core.Pooling;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Movement.Movers;
using Andoco.Unity.Framework.Movement.Objectives;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialSteerNavigator : MonoBehaviour, IMoveNavigator
    {
		private bool isReady;
		private bool isNavigating;
        private readonly Collider[] otherColliders = new Collider[10];
        private Transform tx;
        private IPotentialFieldSystem potentialField;
		private NextStrongestSampleStrategy potentialSampler;
		private IFieldNodeRef currentNode;
		private IFieldNodeRef nextNode;
        private float lastPotentialPosUpdate;
        private Vector3 potentialSteering;
        private Vector3 separationSteering;
        private Vector3 steering;
        private MoverStateKind currentState;

        [Inject]
        private IComponentIndex componentIndex;

        [Inject]
        private IStructuredLog log;

		public string navigatorName = "potentialSteer";
        public Mover mover;

        public float potentialWeight = 1f;
        public PotentialLayerMask potentialLayerMask;
        public float potentialUpdateInterval = 1f;

		public float separationDetectionRadius = 10f;
        public float separationWeight = 1f;
        public LayerMask separationLayers;

		public string Name { get { return navigatorName; } }

		public void StartNavigating()
		{
			isNavigating = true;
		}

		public void StopNavigating()
		{
			isNavigating = false;
		}

        void Awake()
        {
            this.tx = this.transform;

            if (this.mover == null)
                this.mover = this.GetComponent<Mover>();

			this.potentialSampler = new NextStrongestSampleStrategy();
        }

        [Inject]
        void OnPostInject()
        {
            this.Spawned();
        }

        void Spawned()
        {
            this.potentialField = this.componentIndex.GetSingleOrDefault<IPotentialFieldSystem>();
			this.isReady = true;
        }

        void FixedUpdate()
        {
			if (!(this.isReady && this.isNavigating))
				return;
			
            var driver = this.mover.Driver;

            potentialSteering = this.SteerForPotential() * this.potentialWeight;
            separationSteering = this.SteerForSeparation() * this.separationWeight;
            steering = (potentialSteering + separationSteering).normalized;

            // We multiply by speed to ensure it's far enough away for a single frame.
            var newPos = this.tx.position + steering * driver.Speed;

			driver.MoveTo(new Objective(newPos));
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            var p = this.tx.position;

            const float lineLength = 10f;

			if (this.nextNode == null)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawCube(p, Vector3.one * 2f);
			}

			if (this.mover.Driver.CurrentState == MoverStateKind.Arrived)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawCube(p, Vector3.one * 1.5f);
			}

            if (potentialSteering != Vector3.zero)
            {
                var potentialSteeringPos = p + potentialSteering * lineLength;
                Gizmos.color = Color.red;
                Gizmos.DrawLine(p, potentialSteeringPos);
            }
        }

        void Recycled()
        {
			this.currentNode = null;
			this.nextNode = null;
        }

        #region Public methods

        #endregion

        #region Private methods

        private Vector3 SteerForPotential()
        {
			this.currentNode = this.potentialField.GetClosestNode(this.tx.position, this.currentNode);
			var samples = this.potentialField.SamplePotential(this.potentialSampler, this.currentNode, this.potentialLayerMask, null);
			var steer = Vector3.zero;

			if (samples.Count > 0)
			{
				this.nextNode = samples[0].Node;
				steer = (nextNode.Position - this.currentNode.Position).normalized;
			}
			else
			{
				this.nextNode = null;
			}

			samples.ReturnToPool();

			return steer;
        }

        private Vector3 SteerForSeparation()
        {
            var numColliders = Physics.OverlapSphereNonAlloc(this.tx.position, this.separationDetectionRadius, this.otherColliders, this.separationLayers);

            var steering = Vector3.zero;
            var sum = Vector3.zero;

            for (int i = 0; i < numColliders; i++)
            {
                var otherTx = otherColliders[i].GetEntityGameObject().transform;

                if (otherTx == this.tx)
                    continue;

                sum += otherTx.position - this.tx.position;
            }

            sum = sum / numColliders;

            steering = -sum.normalized;

            return steering;
        }

        #endregion
    }
}