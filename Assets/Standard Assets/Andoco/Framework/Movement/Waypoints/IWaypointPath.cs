namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Andoco.Core;

    public delegate void WaypointAddedDelegate(IWaypoint Waypoint);

    public interface IWaypointPath
    {
        event WaypointAddedDelegate OnWaypointAdded;

        IWaypoint this[int index] { get; }

        int Count { get; }

        void Add(IWaypoint waypoint);

        void Clear();

        int GetIndexOf(IWaypoint waypoint);

        int GetCountAhead(IWaypoint waypoint, bool loop = false);

        int GetCountBehind(IWaypoint waypoint, bool loop = false);

        IWaypoint GetNext(IWaypoint waypoint, bool loop = false);

        IWaypoint GetPrevious(IWaypoint waypoint, bool loop = false);

        void RemoveAtIndex(int index);

        bool TryGetAtIndex(int index, out IWaypoint waypoint);

        bool TryGetNext(IWaypoint current, out IWaypoint next, bool loop = false);

        bool TryGetPrevious(IWaypoint current, out IWaypoint previous, bool loop = false);
    }

}