namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

    public static class GizmoHelper
    {
        public static void Cone(Transform originTx, float sweepAngle, float distance)
        {
            var leftRayRotation = Quaternion.AngleAxis(-sweepAngle/2f, originTx.up);
            var rightRayRotation = Quaternion.AngleAxis(sweepAngle/2f, originTx.up);
            var upRayRotation = Quaternion.AngleAxis(sweepAngle/2f, originTx.right);
            var downRayRotation = Quaternion.AngleAxis(-sweepAngle/2f, originTx.right);
            
            Vector3 leftRayDirection = leftRayRotation * originTx.forward;
            Vector3 rightRayDirection = rightRayRotation * originTx.forward;
            Vector3 upRayDirection = upRayRotation * originTx.forward;
            Vector3 downRayDirection = downRayRotation * originTx.forward;
            
            Gizmos.DrawRay(originTx.position, leftRayDirection * distance);
            Gizmos.DrawRay(originTx.position, rightRayDirection * distance);
            Gizmos.DrawRay(originTx.position, upRayDirection * distance);
            Gizmos.DrawRay(originTx.position, downRayDirection * distance);
        }

        public static void Circle(Transform origin, float radius, int segments)
        {
            for (int i = 0; i < segments; i++)
            {
                var a1 = 360f / segments * i;
                var a2 = 360f / segments * (i + 1);

                var p1 = Quaternion.AngleAxis(a1, Vector3.up) * Vector3.forward * radius;
                var p2 = Quaternion.AngleAxis(a2, Vector3.up) * Vector3.forward * radius;

                Gizmos.DrawLine(origin.TransformPoint(p1), origin.TransformPoint(p2));
            }
        }
    }
}
