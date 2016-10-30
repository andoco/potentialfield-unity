namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Andoco.Core;

    public class WaypointPath : IWaypointPath
    {
        private readonly List<IWaypoint> waypoints = new List<IWaypoint>();

        public event WaypointAddedDelegate OnWaypointAdded;

        public IWaypoint this[int index]
        {
            get
            {
                return this.waypoints[index];
            }
        }
        
        public int Count
        {
            get
            {
                return this.waypoints.Count;
            }
        }

        public void Add(IWaypoint waypoint)
        {
//            Debug.LogFormat("Adding waypoint {0}", waypoint);
            this.waypoints.Add(waypoint);

            if (this.OnWaypointAdded != null)
            {
                this.OnWaypointAdded(waypoint);
            }
        }

        public void Clear()
        {
            this.waypoints.Clear();
        }

        public int GetIndexOf(IWaypoint waypoint)
        {
            var index = this.waypoints.IndexOf(waypoint);
            
            if (index == -1)
            {
                throw new InvalidOperationException(string.Format("Could not get the waypoint following {0} because it does not exist in the path", waypoint));
            }
            
            return index;
        }

        public int GetCountAhead(IWaypoint waypoint, bool loop = false)
        {
            if (loop)
                return this.waypoints.Count - 1;
            
            return this.waypoints.Count - (this.GetIndexOf(waypoint) + 1);
        }
        
        public int GetCountBehind(IWaypoint waypoint, bool loop = false)
        {
            if (loop)
                return this.waypoints.Count - 1;
            
            return this.GetIndexOf(waypoint);
        }

        public IWaypoint GetNext(IWaypoint waypoint, bool loop = false)
        {
            IWaypoint next;
            if (!this.TryGetNext(waypoint, out next, loop))
            {
                throw new InvalidOperationException(string.Format("Could not get the waypoint after {0} because there is no next waypoint", waypoint));
            }

            return next;
        }
        
        public IWaypoint GetPrevious(IWaypoint waypoint, bool loop = false)
        {
            IWaypoint previous;
            if (!this.TryGetPrevious(waypoint, out previous, loop))
            {
                throw new InvalidOperationException(string.Format("Could not get the waypoint before {0} because there is no preceeding waypoint", waypoint));
            }
            
            return previous;
        }

        public void RemoveAtIndex(int index)
        {
            this.waypoints.RemoveAt(index);
        }

        public bool TryGetIndexOf(IWaypoint waypoint, out int index)
        {
            index = this.waypoints.IndexOf(waypoint);

            return index >= 0;
        }

        public bool TryGetAtIndex(int index, out IWaypoint waypoint)
        {
            if (this.waypoints.Count == 0 || this.waypoints.Count <= index || index < 0)
            {
                waypoint = null;
                return false;
            }

            waypoint = this.waypoints[index];

            return true;
        }

        public bool TryGetNext(IWaypoint current, out IWaypoint next, bool loop = false)
        {
            int index;
            
            if (this.TryGetIndexOf(current, out index))
            {
                var nextIndex = index + 1;

                if (loop)
                    nextIndex = nextIndex.Wrap(this.Count - 1);
                
                return this.TryGetAtIndex(nextIndex, out next);
            }
            
            next = default(IWaypoint);
            
            return false;
        }

        public bool TryGetPrevious(IWaypoint current, out IWaypoint previous, bool loop = false)
        {
            int index;

            if (this.TryGetIndexOf(current, out index))
            {
                var previousIndex = index - 1;

                if (loop)
                    previousIndex = previousIndex.Wrap(this.Count - 1);
                
                return this.TryGetAtIndex(previousIndex, out previous);
            }

            previous = default(IWaypoint);

            return false;
        }
    }

}