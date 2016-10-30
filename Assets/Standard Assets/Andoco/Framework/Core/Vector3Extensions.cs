namespace Andoco.Unity.Framework.Core
{
	using System;
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.Core;
	using UnityEngine;

	public static class Vector3Extensions
	{
//		public static Vector3 CalculateProjectileTargetPos(this Vector3 origin, Vector3 projectileVelocity, Vector3 targetPos, Vector3 targetVelocity)
//		{
//			Vector3 totarget = targetPos - origin;
//			
//			//float a = Vector3.Dot(targetVelocity, targetVelocity) - (projectileVelocity * projectileVelocity);
//			float a = Vector3.Dot(targetVelocity, targetVelocity) - Vector3.Scale(projectileVelocity, projectileVelocity);
//			float b = 2 * Vector3.Dot(targetVelocity, totarget);
//			float c = Vector3.Dot(totarget, totarget);
//			
//			float p = -b / (2 * a);
//			float q = (float)Mathf.Sqrt((b * b) - 4 * a * c) / (2 * a);
//			
//			float t1 = p - q;
//			float t2 = p + q;
//			float t;
//			
//			if (t1 > t2 && t2 > 0)
//			{
//			    t = t2;
//			}
//			else
//			{
//			    t = t1;
//			}
//			
//			Vector3 aimSpot = target.position + target.velocity * t;
//			
//			return aimSpot;
//		}

		/// <summary>
		/// Restricts the translation applied to a vector relative to an origin point.
		/// </summary>
		/// <returns>The delta by origin.</returns>
		/// <param name="v">The vector being translated.</param>
		/// <param name="origin">The origin point.</param>
		/// <param name="delta">The translation vector being applied to <paramref name="v"/>.</param>
		/// <param name="minDistanceFromOrigin">Minimum allowed distance from origin.</param>
		/// <param name="maxDistanceFromOrigin">Max allowed distance from origin.</param>
		public static Vector3 RestrictDeltaByOrigin(this Vector3 v, Vector3 origin, Vector3 delta, float minDistanceFromOrigin, float maxDistanceFromOrigin)
		{
			var newPos = v + delta;
			
			// perform distance check using x and y axis only
			var p1 = new Vector3(newPos.x, 0, newPos.z);
			var p2 = new Vector3(origin.x, 0, origin.z);
			var d = Vector3.Distance(p1, p2);
			if (d < minDistanceFromOrigin || d > maxDistanceFromOrigin)
				delta = Vector3.zero;

			return delta;
		}

        /// <summary>
        /// Checks if a target vector is within a cone shaped area.
        /// </summary>
        /// <returns><c>true</c> if target is in the cone, <c>false</c> otherwise.</returns>
        /// <param name="origin">The origin vector of the cone.</param>
        /// <param name="direction">The direction in which the cone is facing.</param>
        /// <param name="distance">The distance that the cone beam extends to.</param>
        /// <param name="sweepDegrees">The angle in degrees across the full sweep of the cone.</param>
        /// <param name="target">The target position vector to be checked.</param>
        public static bool CheckInCone(this Vector3 origin, Vector3 direction, float distance, float sweepDegrees, Vector3 target)
        {
            var delta = target - origin;
            
            // Check if within distance of the cone.
            if (delta.magnitude > distance)
                return false;
            
            // Get the heading vector from origin to target.
            var heading = delta.normalized;
            
            // Get the dot product relative to a direction vector from the origin. The result is equal to the cosine of the angle between them.
            // it will be one when the directions are the same, zero when they are at 90º and 0.5 when they are at 60º.
            var dot = Vector3.Dot(direction, heading);
            
            var maxAngleCos = Mathf.Cos(Mathf.Deg2Rad * sweepDegrees / 2f);
            
            if (dot < maxAngleCos)
                return false;
            
            return true;
        }

        /// <summary>
        /// Checks if a target vector is in the direct line of sight of this vector.
        /// </summary>
        /// <returns><c>true</c> if the target is visible, <c>false</c> otherwise.</returns>
        /// <param name="origin">The origin vector.</param>
        /// <param name="target">The target position to be checked.</param>
        /// <param name="layerMask">Layer mask for layers that are considered obstructions to the line of sight.</param>
        public static bool CheckCanSee(this Vector3 origin, Vector3 target, int layerMask)
        {
            var delta = target - origin;
            var ray = new Ray(origin, delta);
            if (Physics.Raycast(ray, delta.magnitude, layerMask))
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Calculates the center of the vertices in <paramref name="vertices"/>.
        /// </summary>
        public static Vector2 CalculateCenter(this IEnumerable<Vector2> vertices)
        {
            var count = 0;
            var sum = Vector2.zero;
            foreach (var p in vertices)
            {
                sum += p;
                count++;
            }
            return sum / count;
        }

		/// <summary>
		/// Calculates the center of the vertices.
		/// </summary>
		public static Vector3 CalculateCenter(params Vector3[] vertices)
		{
			var sum = Vector3.zero;
			for (var i=0; i < vertices.Length; i++)
			{
				sum += vertices[i];
			}
			return sum / vertices.Length;
		}

        /// <summary>
        /// Scales the vertices relative to their center point.
        /// </summary>
        public static void Scale(Vector3[] vertices, float scale)
        {
            var center = Vector3Extensions.CalculateCenter(vertices);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = Vector3.Lerp(vertices[i], center, scale);
            }
        }

        /// <summary>
        /// Recalculate the vertices in the polygon so they are relative to <paramref name="origin"/>.
        /// </summary>
        /// <param name="poly">The polygon whose vertices will be recalculated.</param>
        /// <param name="origin">The origin for the recalculated vertices.</param>
        public static IEnumerable<Vector2> Reposition(this IEnumerable<Vector2> poly, Vector2 origin)
        {
            // Calculate the current center of the polygon.
            var center = poly.CalculateCenter();

            // Calculate the offset from the new origin.
            var offset = origin - center;

            // Apply the offset to the vertices.
            foreach (var p in poly)
            {
                yield return new Vector2(p.x + offset.x, p.y + offset.y);
            }
        }

        public static IEnumerable<Vector2> Scale(this IEnumerable<Vector2> poly, float scale)
        {
            // Calculate the current center of the polygon.
            var center = poly.CalculateCenter();
            var repositioned = poly.Reposition(Vector2.zero);
            var scaled = repositioned.Select(v => v * scale);

            return scaled.Select(v => v + center);
        }

		#region Axis mapping

		/// <summary>
		/// Maps Y to X, Z to Y, and X to Z.
		/// </summary>
		/// <example>
		/// E.g. (1,2,3) => (2,3,1).
		/// </example>
		public static Vector3 ToYZX(this Vector3 v)
		{
			return new Vector3(v.y, v.z, v.x);
		}
		
		/// <summary>
		/// Maps Z to X, X to Y, and Y to Z.
		/// </summary>
		/// <example>
		/// E.g. (1,2,3) => (3,1,2).
		/// </example>
		public static Vector3 ToZXY(this Vector3 v)
		{
			return new Vector3(v.z, v.x, v.y);
		}
		
		/// <summary>
		/// Maps X to X, Z to Y, and Y to Z.
		/// </summary>
		/// <example>
		/// E.g. (1,2,3) => (1,3,2).
		/// </example>
		public static Vector3 ToXZY(this Vector3 v)
		{
			return new Vector3(v.x, v.z, v.y);
		}

		#endregion

        #region Positioning

        [Obsolete("Use SnapToLayer instead")]
        public static Vector3 MatchLayerHeight(this Vector3 pos, int layerMask, Vector3? dir = null, float raycastHeight = 100f, float raise = 0.1f)
        {
            var rayDir = dir.HasValue ? dir.Value : Vector3.down;
            var ray = new Ray(pos + Vector3.up * raycastHeight, rayDir);
            var p = ray.GetHitPoint(layerMask);
            
            return p.HasValue ? p.Value + Vector3.up * raise : pos;
        }

        /// <summary>
        /// Snaps the vector <paramref name="pos"/> to the intersection with a layer in the specified direction.
        /// </summary>
        /// <returns>The new position.</returns>
        /// <param name="pos">The position to snap to the layer.</param>
        /// <param name="layerMask">The layer mask to snap to.</param>
        /// <param name="dir">The direction in which to snap.</param>
        public static Vector3 SnapToLayer(this Vector3 pos, int layerMask, Vector3 dir)
        {
            var ray = new Ray(pos, dir);
            var p = ray.GetHitPoint(layerMask);

            return p.Value;
        }

        /// <summary>
        /// Snaps the vector <paramref name="pos"/> to the intersection with a collider in the specified direction.
        /// </summary>
        /// <returns>The new position.</returns>
        /// <param name="pos">The position to snap to the layer.</param>
        /// <param name="dir">The direction in which to snap.</param>
        /// <param name="config">Configuration of the physics casting to perform.</param>
        public static Vector3 SnapToLayer(this Vector3 pos, Vector3 dir, RaycastConfig config)
        {
            var ray = new Ray(pos, dir);
            RaycastHit hit;

            if (ray.TryCast(out hit, config))
            {
                return hit.point;
            }

            // We didn't hit anything, so return original position.
            return pos;
        }

		/// <summary>
		/// Gets the position of the world point on the 2D screen, constrained to the bounds of the screen rectangle.
		/// </summary>
		/// <returns>The clamped screen position.</returns>
		/// <param name="pos">The world position.</param>
		public static Vector3 ToClampedScreenPos(this Vector3 pos)
		{
			return pos.ToClampedScreenPos(new Rect(0f, 0f, Screen.width, Screen.height));
		}

		/// <summary>
		/// Gets the position of the world point on the 2D screen, constrained to the bounds of the screen rectangle.
		/// </summary>
		/// <returns>The clamped screen position.</returns>
		/// <param name="pos">The world position.</param>
		/// <param name="screenRect">The screen rectangle to clamp to.</param> 
		public static Vector3 ToClampedScreenPos(this Vector3 pos, Rect screenRect)
		{
			var screenPos = Camera.main.WorldToScreenPoint(pos);
			
			// If the object is on the other side of the camera plane (i.e. behind it), then reverse the coordinates.
			if (screenPos.z < 0f)
				screenPos = -screenPos;
			
			var clampedScreenPos = new Vector3(Mathf.Clamp(screenPos.x, screenRect.x, screenRect.width), Screen.height - Mathf.Clamp(screenPos.y, screenRect.y, screenRect.height), screenPos.z);

			return clampedScreenPos;
		}

		/// <summary>
		/// Determines if the world point is within the visible screen space.
		/// </summary>
		/// <returns><c>true</c> if is in screen bounds; otherwise, <c>false</c>.</returns>
		/// <param name="pos">The world position.</param>
		public static bool IsInScreenBounds(this Vector3 pos)
		{
			var screenRect = new Rect(0f, 0f, Screen.width, Screen.height);
			var screenPos = Camera.main.WorldToScreenPoint(pos);

			return screenRect.Contains(screenPos);
		}

        #endregion

        #region Origin based methods

        public static Vector3 RadialPoint(this Vector3 origin, float degrees, float radius)
        {
            var rot = Quaternion.AngleAxis(degrees, Vector3.up);
            var localPos = rot * (Vector3.right * radius);
            
            return origin + localPos;
        }
        
        public static Vector3 RadialPoint(this Vector3 origin, int segments, int segmentIdx, float radius)
        {
            var rot = Quaternion.AngleAxis(360f / segments * segmentIdx, Vector3.up);
            var localPos = rot * (Vector3.right * radius);
            
            return origin + localPos;
        }

        /// <summary>
        /// Returns the position of <paramref name="point"/> after being rotated around a pivot point.
        /// </summary>
        /// <returns>The around.</returns>
        /// <param name="point">The point to be rotated.</param>
        /// <param name="origin">The pivot point to rotate around.</param>
        /// <param name="angles">The rotation vector to perform.</param>
        public static Vector3 RotateAround(this Vector3 point, Vector3 origin, Vector3 angles)
        {
            return point.RotateAround(origin, Quaternion.Euler(angles));
        }

        /// <summary>
        /// Returns the position of <paramref name="point"/> after being rotated around a pivot point.
        /// </summary>
        /// <returns>The around.</returns>
        /// <param name="point">The point to be rotated.</param>
        /// <param name="origin">The pivot point to rotate around.</param>
        /// <param name="rotation">The rotation to perform.</param>
        public static Vector3 RotateAround(this Vector3 point, Vector3 pivot, Quaternion rotation)
        {
            return pivot + (rotation * (point - pivot));
        }

        /// <summary>
        /// Gets the angle between <paramref name="from"/> and <paramref name="to"/> based on the direction of rotation <paramref name="direction"/>.
        /// </summary>
        /// <remarks>
        /// (0,0,1) to (1,0,0) in a clockwise direction would be 90 deg.
        /// (1,0,0) to (0,0,1) in an anticlockwise direction would also be 90 deg.
        /// </remarks>
        /// <returns>The angle between the two points relative to the supplied direction.</returns>
        /// <param name="origin">Origin.</param>
        /// <param name="from">The position from which to start the angle.</param>
        /// <param name="to">The position from which to end the angle.</param>
        /// <param name="normal">The axis of rotation.</param>
        /// <param name="direction">The direction of rotation around the axis.</param>
        public static float GetAngleInDirection(this Vector3 origin, Vector3 from, Vector3 to, Vector3 normal, RotationDirectionKind direction)
        {
            float angle;
            
            switch (direction)
            {
                case RotationDirectionKind.None:
                case RotationDirectionKind.Clockwise:
                    angle = Math3d.SignedVectorAngle(from - origin, to - origin, normal);
                    break;
                case RotationDirectionKind.Anticlockwise:
                    angle = Math3d.SignedVectorAngle(to - origin, from - origin, normal);
                    break;
                default:
                    throw new System.InvalidOperationException(string.Format("Unknown {0} value {1}", typeof(RotationDirectionKind), direction));
            }
            
            return angle;
        }

        /// <summary>
        /// Calculates the arc distance on a sphere of <paramref name="radius"/> between the two points <paramref name="from"/> and <paramref name="to"/>.
        /// </summary>
        /// <remarks>
        /// Only the direction of the <paramref name="from"/> and <paramref name="to"/> vectors relative to the <paramref name="origin"/>
        /// will be used in the calculation. Their magnitude will be overriden by the supplied <paramref name="radius"/>.
        /// 
        /// See https://en.wikipedia.org/wiki/Great-circle_distance
        /// </remarks>
        /// <returns>The arc distance between the points.</returns>
        /// <param name="origin">The point considered as the origin of the sphere.</param>
        /// <param name="from">The starting point.</param>
        /// <param name="to">The end point.</param>
        /// <param name="radius">The radius of the sphere we will calculate the distance on.</param>
        public static float GetArcDistance(this Vector3 origin, Vector3 from, Vector3 to, float radius)
        {
            var v1 = from - origin;
            var v2 = to - origin;

            var a = Vector3.Angle(v1, v2);

            var d = (2f * Mathf.PI * radius) * (a / 360f);

            return d;
        }

        #endregion

        #region Conversion

        /// <summary>
        /// Converts a cartesian coordinate (x,y,z) into a 2D polar coordinate.
        /// </summary>
        /// <remarks>>
        /// Taken from http://answers.unity3d.com/questions/189724/polar-spherical-coordinates-to-xyz-and-vice-versa.html.
        /// 
        /// The direction in which coordinates get mapped is controlled by the default rotation direction when using
        /// Unity's math methods.
        /// </remarks>
        /// <returns>The polar coordinate.</returns>
        /// <param name="cartesian">The cartesian coordinate.</param>
        public static Vector2 CartesianToPolar(this Vector3 point)
        {
            Vector2 polar;

            //calc longitude
//            polar.y = Mathf.Atan2(point.x,point.z);

            // This works
            polar.y = Mathf.Atan2(-point.z,point.x);
            
            //this is easier to write and read than sqrt(pow(x,2), pow(y,2))!
            var xzLen = new Vector2(point.x,point.z).magnitude; 
            //atan2 does the magic
            polar.x = Mathf.Atan2(-point.y,xzLen);

            //convert to deg
            polar *= Mathf.Rad2Deg;

            return polar;
        }

        #endregion
	}
}
