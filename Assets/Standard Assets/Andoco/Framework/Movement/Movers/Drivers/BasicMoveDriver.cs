namespace Andoco.Unity.Framework.Movement.Movers.Drivers
{
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Movement.Objectives;
    using UnityEngine;
    using Zenject;

    public class BasicMoveDriver : MonoBehaviour, IMoveDriver
    {
        private Transform cachedTx;
        private Queue<IObjective> destinations = new Queue<IObjective>();
        private IObjective previousObjective;
        private bool paused;
        private MoverStateKind currentState;

        public string driverName;
        public float startSpeed = 1f;
        public float arrivalDistance = 0.1f;
        [Tooltip("If set, the mover will continue in the Moving state when the arrival distance is reached. Useful for following targets.")]
        public bool ignoreArrival;
        public MoverLookMode lookMode;
        public bool snapDown;
        public LayerMask snapLayer;
        public float raiseSnapRaycast;
        public float raiseSnapPosition;

        #region Events

        public event ObjectiveChangedDelegate ObjectiveChanged;

        public event MoverStateChangedDelegate StateChanged;

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                return this.driverName;
            }
        }

        public float Speed { get; set; }

        public MoverStateKind CurrentState
        {
            get
            {
                return this.currentState;
            }
            set
            {
                if (this.currentState != value)
                {
                    var previousState = this.currentState;
                    this.currentState = value;
                    if (this.StateChanged != null)
                        this.StateChanged(this, previousState, this.currentState);
                }
            }
        }

        #endregion

        #region Lifecycle

        void Awake()
        {
            this.cachedTx = this.transform;
            this.Speed = this.startSpeed;
        }

        void FixedUpdate()
        {
            if (this.paused)
                return;
            
            IObjective destination;

            if (this.TryPeekDestination(out destination))
            {
                if (this.CurrentState != MoverStateKind.Moving)
                {
                    // Raise an event when we start moving for the first time.
                    this.RaiseObjectiveChanged(this.previousObjective, destination);
                    this.CurrentState = MoverStateKind.Moving;
                }

                // We need to modify the arrival distance to take account of the raised snap position.
                // TODO: This probably isn't accurate. Should raise the destination position first.
                var raisedArrialDistance = this.arrivalDistance + this.raiseSnapPosition;
                
                if (this.cachedTx.MoveTowards(destination.TargetPosition, this.Speed, raisedArrialDistance) && !this.ignoreArrival)
                {
                    this.destinations.Dequeue();

                    if (this.destinations.Count == 0)
                    {
                        this.CurrentState = MoverStateKind.Arrived;
                    }

                    this.RaiseObjectiveChanged(destination, this.destinations.Count > 0 ? this.destinations.Peek() : null);

                    this.previousObjective = destination;
                }

                // TODO: Integrate orientation in here somehow? Maybe using movement modifiers, and a OnPostMove callback.

                if (this.snapDown)
                {
                    var rayconf = RaycastConfig.Ray(layerMask: this.snapLayer);
                    this.cachedTx.SnapDown(rayconf, this.raiseSnapRaycast, this.raiseSnapPosition);
                }

                switch (this.lookMode)
                {
                    case MoverLookMode.None:
                        break;
                    case MoverLookMode.WorldUp:
                        this.cachedTx.LookAt(destination.TargetPosition);
                        break;
                    case MoverLookMode.TransformUp:
                        this.cachedTx.LookAt(destination.TargetPosition, this.cachedTx.up);
                        break;
                }
            }
            else
            {
                if (this.CurrentState == MoverStateKind.Moving)
                {
                    this.Cancel();
                }
            }
        }

        void Recycled()
        {
            this.Cancel();
            this.Clear();
        }

        #endregion

        #region IMoveDriver implementation

        public void SetActiveDriver(bool isActive)
        {
        }

        public void Cancel()
        {
            this.destinations.Clear();
            this.previousObjective = null;
            this.CurrentState = MoverStateKind.Cancelled;
        }

        public void MoveTo(IObjective destination)
        {
            this.destinations.Clear();
            this.previousObjective = null;
            this.destinations.Enqueue(destination);
        }

        public void EnqueueMoveTo(IObjective destination)
        {
            this.destinations.Enqueue(destination);
        }

        public void Pause()
        {
            this.paused = true;
            this.CurrentState = MoverStateKind.Paused;
        }

        public void Play()
        {
            this.paused = false;
        }

        public void Clear()
        {
            this.destinations.Clear();
            this.previousObjective = null;
            this.paused = false;
            this.CurrentState = MoverStateKind.None;
            this.Speed = this.startSpeed;
        }

        #endregion

        #region Private methods

        private bool TryPeekDestination(out IObjective destination)
        {
            if (this.destinations.Count == 0)
            {
                destination = null;
                return false;
            }

            destination = this.destinations.Peek();

            // Dequeue objectives until we have a valid one.
            while (!destination.Validate())
            {
                this.destinations.Dequeue();

                if (this.destinations.Count > 0)
                {
                    destination = this.destinations.Peek();
                }
                else
                {
                    destination = null;
                    return false;
                }
            }

            return true;
        }

        private void RaiseObjectiveChanged(IObjective previousObjective, IObjective currentObjective)
        {
            if (this.ObjectiveChanged != null)
            {
                this.ObjectiveChanged(this, previousObjective, currentObjective);
            }
        }

        #endregion
    }
}