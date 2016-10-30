namespace Andoco.Unity.Framework.Movement.Movers.Drivers
{
    using UnityEngine;
    using System.Collections.Generic;
    using Andoco.Core;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Movement.Objectives;
    using Andoco.Unity.Framework.Movement.Waypoints;
    using Zenject;

    public class WaypointPathMoveDriver : MonoBehaviour, IMoveDriver
    {
        private const string LogDestinationKey = "destination";
        private const string LogCurrentStateKey = "currentState";

        [Inject]
        private IStructuredLog log;

        private IWaypointPath path;

        private bool paused;

        private MoverStateKind currentState;
        
        public string driverName;
        
        public WaypointPathMoverLookMode lookMode;

        public bool loop;

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

        public IWaypointPathMover PathMover { get; private set; }
        
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
            this.path = new WaypointPath();
            this.PathMover = this.GetComponent<IWaypointPathMover>();
        }
        
        void Update()
        {
            if (!this.paused && this.path.Count > 0)
            {
                this.CurrentState = MoverStateKind.Moving;

                this.SyncPathMover();

                if (this.PathMover.IsArrived)
                {
                    this.CurrentState = MoverStateKind.Arrived;
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
            if (isActive)
            {
                this.SyncPathMover();
                this.PathMover.Follow(this.path);
                this.PathMover.OnWaypointChanged += this.OnWaypointChanged;
            }
            else
            {
                this.PathMover.OnWaypointChanged -= this.OnWaypointChanged;
            }
        }
        
        public void Cancel()
        {
            this.PathMover.Stop();
            this.path.Clear();
            this.CurrentState = MoverStateKind.Cancelled;
        }
        
        public void MoveTo(IObjective destination)
        {
            if (this.log.Status.IsInfoEnabled)
                this.log.Trace("Will move to new destination on new path", x => x
                    .Field(LogDestinationKey, destination)
                    .Field(LogCurrentStateKey, this.CurrentState));
            
            this.PathMover.Stop();
            this.path.Clear();
            this.path.Add(this.CreateWaypoint(destination));
        }

        public void EnqueueMoveTo(IObjective destination)
        {
            if (this.log.Status.IsInfoEnabled)
                this.log.Trace("Adding destination to existing path", x => x
                    .Field(LogDestinationKey, destination)
                    .Field(LogCurrentStateKey, this.CurrentState));
            
            this.path.Add(this.CreateWaypoint(destination));
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
            this.path.Clear();
            this.paused = false;
            this.CurrentState = MoverStateKind.None;
        }
        
        #endregion

        #region Private methods

        private IWaypoint CreateWaypoint(IObjective objective)
        {
            // TODO: Use a pool to avoid too many allocations.
            return new ObjectiveWaypoint(objective);
        }

        private void OnWaypointChanged(IWaypoint previousWaypoint, IWaypoint currentWaypoint)
        {
            if (this.ObjectiveChanged != null)
            {
                var previousObjective = previousWaypoint == null
                    ? null
                    : ((ObjectiveWaypoint)previousWaypoint).Objective;

                var currentObjective = currentWaypoint == null
                    ? null
                    : ((ObjectiveWaypoint)currentWaypoint).Objective;
                
                this.ObjectiveChanged(this, previousObjective, currentObjective);
            }
        }

        private void SyncPathMover()
        {
            this.PathMover.Speed = this.Speed;
            this.PathMover.Loop = this.loop;
            this.PathMover.LookMode = this.lookMode;
        }

        #endregion
    }
}