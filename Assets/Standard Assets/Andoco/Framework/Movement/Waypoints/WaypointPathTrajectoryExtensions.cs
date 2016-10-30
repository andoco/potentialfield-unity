namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using UnityEngine;
    using Andoco.Unity.Framework.Core;

    public static class WaypointPathTrajectoryExtensions
    {
        public static void AddSpiral(this IWaypointPath path, Vector3 startPos, Vector3 pivot, float endDistance, float endHeight, float stepAngle = 10f, int numSteps = 36)
        {
            var offset = startPos - Math3d.ProjectPointOnLine(pivot, Vector3.up, startPos);
            var dir = offset.normalized;
            
            var startD = offset.magnitude;
            
            var startH = startPos.y;
            
            var p = startPos;

            for (var i = 0; i < numSteps; i++)
            {
                var t = 1f / numSteps * i;
                var d = Mathf.Lerp(startD, endDistance, t);
                var h = Mathf.Lerp(startH, endHeight, t);
                
                p = Quaternion.Euler(0f, i * stepAngle, 0f) * dir * d;
                p += Vector3.up * h;

                path.Add(new Waypoint(p));
            }
        }
    }
}
