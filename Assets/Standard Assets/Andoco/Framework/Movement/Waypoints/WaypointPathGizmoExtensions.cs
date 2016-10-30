namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using UnityEngine;

    public static class WaypointPathGizmoExtensions
    {
        public static void DrawGizmos(this IWaypointPath path)
        {
            Gizmos.color = Color.grey;

            for (var i = 0; i < path.Count; i++)
            {
                Gizmos.DrawSphere(path[i].Position, 0.25f);
            }
        }
    }
}
