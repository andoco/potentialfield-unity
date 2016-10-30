namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.Core;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public class CatmullRomWaypointPathMover : MonoBehaviour, IWaypointPathMover
    {
        private const string LogNumWaypointsKey = "numWaypoints";
        private const string LogWaypointKey = "waypoint";

        [Inject]
        private IStructuredLog logger;

        private WaypointPathMoverFsm fsm;
        private Transform cachedTx;
        private Vector3 startPos;
        private List<Vector3> tmp;

        public bool loggingEnabled;

        void Awake()
        {
            this.cachedTx = transform;
            this.tmp = new List<Vector3>();

            this.fsm = new WaypointPathMoverFsm();
            this.fsm.ShouldStartMovement = this.ShouldStartMovment;
            this.fsm.StartMovement = this.StartMovement;
            this.fsm.StopMovement = this.StopMovement;
            this.fsm.RecalculateMovement = this.RecalculateMovement;
        }

        public event WaypointDelegate OnWaypointChanged;

        public IWaypointPath Path { get; private set; }

        public IWaypoint Current { get; protected set; }

        public IWaypoint Previous { get; protected set; }

        public float Speed { get; set; }

        public bool Loop { get; set; }

        public bool IsMoving
        {
            get
            {
                return this.fsm.CurrentState == WaypointPathMoverFsm.FsmStateKind.Moving;
            }
        }

        public bool IsArrived
        {
            get
            {
                return this.fsm.CurrentState == WaypointPathMoverFsm.FsmStateKind.Idle && this.Path.IsLast(this.Previous);
            }
        }

        public WaypointPathMoverLookMode LookMode { get; set; }

        public List<Vector3> curvePoints = new List<Vector3>();

        public List<Vector3> vectorPath = new List<Vector3>();

        #region Lifecycle

        void Update()
        {
            // Find the max distance we can travel this tick.
            var maxDistance = this.Speed * Time.deltaTime;
            var remainingMovableDist = maxDistance;

            while (this.Current != null && remainingMovableDist > 0f)
            {
                if (this.Current != null)
                {
                    if (this.vectorPath.Count == 0)
                    {
                        this.BuildVectorPath();
                    }

                    var targetPos = this.vectorPath[0];
                    var distToTarget = (targetPos - this.cachedTx.position).magnitude;
                    var distToTravel = Mathf.Min(distToTarget, remainingMovableDist);
                    var arrived = distToTravel == distToTarget;

                    this.cachedTx.position = Vector3.MoveTowards(this.cachedTx.position, targetPos, distToTravel);

                    switch (this.LookMode)
                    {
                        case WaypointPathMoverLookMode.None:
                            break;
                        case WaypointPathMoverLookMode.WorldUp:
                            this.cachedTx.LookAt(targetPos);
                            break;
                        case WaypointPathMoverLookMode.TransformUp:
                            this.cachedTx.LookAt(targetPos, this.cachedTx.up);
                            break;
                    }

                    remainingMovableDist -= distToTravel;

                    if (arrived)
                    {
                        if (this.loggingEnabled && logger.Status.IsTraceEnabled)
                            logger.Trace("Arrived at vectorPath point", x => x.Field("vectorPathPoint", this.vectorPath[0]).Field("remainingVectorPathPoints", this.vectorPath.Count - 1));

                        // We've reached the end of a curve slice, so move to the next one.
                        // TODO: Pop from the end.
                        this.vectorPath.RemoveAt(0);
                    }

                    if (arrived && this.vectorPath.Count == 0)
                    {
                        // We've arrived at the current waypoint.

                        if (this.loggingEnabled && logger.Status.IsTraceEnabled)
                            logger.Trace("Arrived at waypoint", x => x.Field("current", this.Current));

                        this.Previous = this.Current;

                        IWaypoint next;
                        if (this.Path.TryGetNext(this.Current, out next))
                        {
                            this.Current = next;
                            this.WaypointChanged(this.Previous, this.Current);
                        }
                        else
                        {
                            // We've reached the end of the path.
                            if (this.Loop)
                            {
                                this.Current = this.Path[0];
                                this.WaypointChanged(this.Previous, this.Current);
                            }
                            else
                            {
                                this.Current = null;
                                this.WaypointChanged(this.Previous, this.Current);
                                this.fsm.TriggerArrived();
                            }
                        }

                        if (this.loggingEnabled && logger.Status.IsTraceEnabled)
                            logger.Trace("Next waypoint assigned as current waypoint", x => x.Field("current", this.Current));

                        this.vectorPath.Clear();
                    }
                }
            }
        }

        #endregion

        #region Public methods

        public void Follow(IWaypointPath path)
        {
            this.Path = path;

            this.Path.OnWaypointAdded += waypoint =>
            {
//                this.WaypointAdded(waypoint);

                this.logger.Trace("New waypoint added to path", x => x
                    .Field(LogNumWaypointsKey, this.Path.Count)
                    .Field(LogWaypointKey, waypoint));

                // TODO: probably don't need to explicity check the current fsm state. It will respect the transition table.
                if (this.fsm.CurrentState == WaypointPathMoverFsm.FsmStateKind.Idle)
                {
                    this.fsm.TriggerWaypointAdded();
                }
            };            
        }

        public void Stop()
        {
            this.fsm.Trigger(WaypointPathMoverFsm.FsmEventKind.Stop, true);
        }

        #endregion

        #region Private methods

        private bool ShouldStartMovment()
        {
            if (this.Loop)
            {
                return this.Path.Count > 3;
            }

            // We can start whenever there are waypoints in the path and we haven't
            // already visited the final waypoint.
            var willStart = this.Path.Count > 0 && (this.Previous == null || this.Path.HasNext(this.Previous));

            if (!willStart)
                this.logger.Trace("Will not start movement because path evaluation failed", x => x
                    .Field(LogNumWaypointsKey, this.Path.Count));

            return willStart;
        }

        private void StartMovement()
        {
            System.Diagnostics.Debug.Assert(this.Path.Count > 0, "Path too short");

            if (this.Previous == null)
            {
                // We're at the start of the path.
                this.Current = this.Path[0];
            }
            else
            {
                // We're somewhere in the middle of the path and there should be more waypoints ahead.
                IWaypoint next;
                if (this.Path.TryGetNext(this.Previous, out next))
                {
                    this.Current = next;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Cannot start movement because no waypoint could be found after the previous waypoint {0}", this.Previous));
                }

            }
        }

        private void RecalculateMovement()
        {
            Debug.LogWarning("Should be no need to RecalculateMovement");
        }

        private void StopMovement()
        {
            this.Current = null;
            this.Previous = null;

            this.curvePoints.Clear();
            this.vectorPath.Clear();
        }

        private void WaypointChanged(IWaypoint previousWaypoint, IWaypoint currentWaypoint)
        {
            System.Diagnostics.Debug.Assert(previousWaypoint != this.Current, "Current waypoint should not be the same as the visited waypoint");

            this.RaiseWaypointChanged(previousWaypoint, currentWaypoint);
        }

        private void RaiseWaypointChanged(IWaypoint previousWaypoint, IWaypoint currentWaypoint)
        {
            if (this.OnWaypointChanged != null)
            {
                this.OnWaypointChanged(previousWaypoint, currentWaypoint);
            }
        }

        private void BuildVectorPath()
        {
            System.Diagnostics.Debug.Assert(this.tmp.Count == 0, "The list should have been cleared");
            System.Diagnostics.Debug.Assert(this.Current != null, "There should be a current waypoint");

            const int slices = 10;

            this.curvePoints.Clear();

            this.FillCatmullRomControlPoints(this.tmp);
            this.FillCatmullRom(this.tmp, this.curvePoints, slices, false);

            this.tmp.Clear();

            for (int i = 0; i < this.curvePoints.Count; i++)
            {
                this.vectorPath.Add(this.curvePoints[i]);
            }
        }

        private bool FillCatmullRomControlPoints(List<Vector3> tmp)
        {
            Vector3 p0, p1, p2, p3;

            p0 = this.GetPointBehind();
            p1 = this.cachedTx.position;
            p2 = this.Current.Position;
            p3 = this.GetPointAhead();

            tmp.Add(p0);
            tmp.Add(p1);
            tmp.Add(p2);
            tmp.Add(p3);

            return true;
        }

        private Vector3 GetPointBehind()
        {
            IWaypoint previous, behindPrevious;

            if (
                this.Path.TryGetPrevious(this.Current, out previous, this.Loop) &&
                this.Path.TryGetPrevious(previous, out behindPrevious, this.Loop))
            {
                return behindPrevious.Position;
            }

            var d = Vector3.Distance(this.cachedTx.position, this.Current.Position);

            return this.cachedTx.TransformPoint(Vector3.back * d);
        }

        private Vector3 GetPointAhead()
        {
            IWaypoint ahead;

            if (this.Path.TryGetNext(this.Current, out ahead, this.Loop))
            {
                return ahead.Position;
            }

            return this.Current.Position + (this.Current.Position - this.cachedTx.position).normalized;
        }

        private void FillCatmullRom(List<Vector3> srcPoints, List<Vector3> dstPoints, int slices, bool loop)
        {
            Assert.IsTrue(srcPoints.Count >= 4, "Not enough source points");
            Assert.IsTrue(slices > 0, "Not enough slices");

            var sliceTime = 1f / slices;

            for (int i = 1; i < srcPoints.Count - 2; i++)
            {
                for (int j = 0; j <= slices; j++)
                {
                    var t = sliceTime * j;
                    var p = VectorHelper.CatmullRom(t, srcPoints[i - 1], srcPoints[i], srcPoints[i + 1], srcPoints[i + 2]);
                    dstPoints.Add(p);
                }
            }
        }

        #endregion
    }
}
