namespace Andoco.Unity.Framework.Core.Scene.Inspection
{
    using System;
    using UnityEngine;

    public class CircleBoundaryDetector : ICircleBoundaryDetector
    {
        public CircleBoundaryDetector()
            : this(LayerMaskConstants.LayerMaskAll, 0.1f)
        {
        }

        public CircleBoundaryDetector(int layerMask, float overlapRadius)
        {
            this.LayerMask = layerMask;
            this.OverlapRadius = overlapRadius;
        }

        public int LayerMask { get; set; }

        public float OverlapRadius { get; set; }

        public float Detect(Vector3 origin, Vector3 normal, float startRadius, float radiusStep, int maxAttempts = -1)
        {
            if (radiusStep <= 0f)
            {
                throw new ArgumentOutOfRangeException("radiusStep", radiusStep, "radiusStep must be greater than zero");
            }

            // Default to 1000 attempts.
            if (maxAttempts == -1)
                maxAttempts = 1000;

            var r = startRadius;
            var numSegments = 8;
            var angleStep = 360f / numSegments;
            var attemptsRemaining = maxAttempts;
            
            while (true)
            {
//                Debug.LogFormat("Testing at origin = {0}, radius = {1}", origin, r);
                
                attemptsRemaining--;
                
                var hit = false;
                
                for (var i = 1; i <= numSegments; i++)
                {
                    var rot1 = Quaternion.AngleAxis(angleStep * (i - 1), normal);
                    var p1 = origin + (rot1 * (Vector3.forward * r));
                    
                    var rot2 = Quaternion.AngleAxis(angleStep * i, normal);
                    var p2 = origin + (rot2 * (Vector3.forward * r));

                    var diff = p2 - p1;
                    var ray = new Ray(p1, diff.normalized);

                    // Check if either point is inside a collider, or if there is no line of sight between them.
                    if (
                        Physics.CheckSphere(p1, this.OverlapRadius, this.LayerMask) ||
                        Physics.CheckSphere(p2, this.OverlapRadius, this.LayerMask) ||
                        Physics.Raycast(ray, diff.magnitude, this.LayerMask))
                    {
                        hit = true;
                        break;
                    }
                }
                
                if (hit)
                {
//                    Debug.LogFormat("hit at radius {0}", r);
                    // We increase the radius by one step so we're using the last known enclosing radius.
                    r += radiusStep;
                    break;
                }
                else
                {
//                    Debug.LogFormat("no hit at radius {0}", r);
                    r -= radiusStep;
                }
                
                if (attemptsRemaining == 0 || r <= 0f)
                {
//                    Debug.LogError("Failed to detect radius");
                    r = float.NaN;
                    break;
                }
            }
            
//            Debug.LogFormat("Detected radius = {0}", r);
            
            return r;
        }
    }
}
