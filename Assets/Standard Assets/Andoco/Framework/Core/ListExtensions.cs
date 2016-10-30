namespace Andoco.Unity.Framework.Core
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using UnityEngine;

	public static class ListExtensions
	{
	    /// <summary>
	    /// Calculates the entire distance along a path of points.
	    /// </summary>
	    /// <returns>The total distance.</returns>
	    /// <param name="points">Path of points.</param>
	    public static float CalculateDistance(this IList<Vector3> points)
	    {
	        var dist = 0f;

	        for (int i=0; i < points.Count - 1; i++)
	            dist += Vector3.Distance(points[i], points[i+1]);
	        
	        return dist;
	    }

        /// <summary>
        /// Finds the index of the vector in the list that is closest to a point.
        /// </summary>
        /// <returns>The index of the closest vector.</returns>
        /// <param name="points">The list of points to compare against.</param>
        /// <param name="pos">The position to compare against.</param>
        public static int FindClosestIndex(this IList<Vector3> points, Vector3 pos)
        {
            var closestIdx = 0;
            var closestDist = float.MaxValue;

            for (int i=0; i < points.Count; i++)
            {
                var p = points[i];
                var d = Vector3.Distance(pos, p);
                if (d < closestDist)
                {
                    closestIdx = i;
                    closestDist = d;
                }
            }
            
            return closestIdx;
        }
	}
}