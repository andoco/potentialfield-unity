namespace Andoco.Unity.Framework.Movement.Waypoints
{
    public static class WaypointPathExtensions
    {
        public static bool IsLast(this IWaypointPath path, IWaypoint waypoint)
        {
            if (path.Count > 0 && waypoint == path[path.Count - 1])
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the specified waypoint has another waypoint after it.
        /// </summary>
        /// <returns><c>true</c> if there is a next waypoint, or if the specified waypoint is null; otherwise, <c>false</c>.</returns>
        /// <param name="path">The path to check.</param>
        /// <param name="waypoint">The waypoint to check for a next waypoint.</param>
        public static bool HasNext(this IWaypointPath path, IWaypoint waypoint)
        {
            IWaypoint next;
            return path.TryGetNext(waypoint, out next);
        }

        /// <summary>
        /// Removes all waypoints on the path ahead of <paramref name="waypoint"/>.
        /// </summary>
        /// <param name="path">The current waypoint path.</param>
        /// <param name="waypoint">The waypoint from which to remove waypoints from.</param>
        public static void TrimAhead(this IWaypointPath path, IWaypoint waypoint)
        {
            var startIndex = path.GetIndexOf(waypoint);

            for (var i = startIndex + 1; i < path.Count; i++)
            {
                path.RemoveAtIndex(i);
            }
        }
    }
}