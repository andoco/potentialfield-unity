namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using UnityEngine;

    public static class CatmullRomWaypointPathMoverGizmoExtensions
    {
        public static void DrawGizmos(this CatmullRomWaypointPathMover mover)
        {
            Gizmos.color = Color.yellow;

            for (var i = 0; i < mover.vectorPath.Count; i++)
            {
                Gizmos.DrawSphere(mover.vectorPath[i], 0.15f);
            }
            
            Gizmos.color = Color.cyan;

            for (var i = 0; i < mover.curvePoints.Count; i++)
            {
                Gizmos.DrawSphere(mover.curvePoints[i], 0.1f);
            }
        }
    }
}
