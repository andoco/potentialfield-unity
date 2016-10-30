namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;
    using System.Collections.Generic;

    public static class VectorHelper
    {
        /// <summary>
        /// Gets a number of evenly distributed points on the surface of a sphere.
        /// </summary>
        /// <remarks>>
        /// Originally taken from http://forum.unity3d.com/threads/evenly-distributed-points-on-a-surface-of-a-sphere.26138/
        /// </remarks>
        /// <returns>The points on sphere.</returns>
        /// <param name="numPoints">The number of points to calculate.</param>
        public static Vector3[] GetPointsOnSphere(int numPoints)
        {
            var upts = new Vector3[numPoints];
            float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
            float off = 2.0f / numPoints;
            float x = 0;
            float y = 0;
            float z = 0;
            float r = 0;
            float phi = 0;

            for (var k = 0; k < numPoints; k++){
                y = k * off - 1 + (off /2);
                r = Mathf.Sqrt(1 - y * y);
                phi = k * inc;
                x = Mathf.Cos(phi) * r;
                z = Mathf.Sin(phi) * r;

                upts[k] = new Vector3(x, y, z);
            }

            return upts;
        }

        public static Vector3 CatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 a = 0.5f * (2f * p1);
            Vector3 b = 0.5f * (p2 - p0);
            Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
            Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

            Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

            return pos;
        }
    }
}