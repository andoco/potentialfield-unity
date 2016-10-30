namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;

    public delegate void WaypointDelegate(IWaypoint previousWaypoint, IWaypoint currentWaypoint);

    public interface IWaypointPathMover
    {
        /// <summary>
        /// Raised when visiting a waypoint, after the <see cref="Current"/> property has been changed to the next waypoint.
        /// </summary>
        event WaypointDelegate OnWaypointChanged;

        /// <summary>
        /// Gets the waypoint that is the current movement destination. <c>null</c> if movement not started, or if at the end of the path.
        /// </summary>
        IWaypoint Current { get; }

        /// <summary>
        /// Gets the most recently visited waypoint. <c>null</c> if no waypoints have yet been visited.
        /// </summary>
        IWaypoint Previous { get; }

        float Speed { get; set; }

        bool Loop { get; set; }

        bool IsMoving { get; }

        bool IsArrived { get; }
        
        WaypointPathMoverLookMode LookMode { get; set; }

        void Follow(IWaypointPath path);

        void Stop();
    }
}
