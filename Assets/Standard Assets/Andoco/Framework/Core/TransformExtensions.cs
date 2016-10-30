namespace Andoco.Unity.Framework.Core
{
	using System;
	using UnityEngine;

	public static class TransformExtensions
	{
		public static Vector3 PredictFuturePosition(this Transform transform, float speed, float time)
		{
			return transform.position + (transform.forward * speed * time);
		}
		
		/// <summary>
		/// Calculates the time in seconds required to reach <paramref name="targetPos"/> at <paramref name="speed"/>.
		/// </summary>
		/// <remarks>
		/// The calculation assumes that the <paramref name="targetPos"/> remains stationary.
		/// </remarks>
		/// <returns>
		/// The time in seconds to reach <paramref name="targetPos"/>.
		/// </returns>
		/// <param name='transform'>
		/// Transform.
		/// </param>
		/// <param name='targetPos'>
		/// Target position.
		/// </param>
		/// <param name='speed'>
		/// Speed.
		/// </param>
		public static float TimeToTarget(this Transform transform, Vector3 targetPos, float speed)
		{
			var distance = Vector3.Distance(targetPos, transform.position);
			
			return distance / speed;
		}

		public static bool MoveTowards(this Transform transform, Vector3 targetPos, float speed, float arrivalDist)
		{
            var newPos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            transform.position = newPos;

            return (targetPos - newPos).sqrMagnitude <= (arrivalDist * arrivalDist);
		}

	    public static bool IsHit(this Transform target, Vector3 screenPos)
	    {
	        Ray ray;
	        RaycastHit hit;
	        
	        ray = Camera.main.ScreenPointToRay(screenPos);
	        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
	        {
	            if (hit.collider.transform == target)
	            {
	                return true;
	            }
	        }
	        
	        return false;
	    }

	    public static bool IsNearHit(this Transform target, Vector3 screenPos, float radius)
	    {
	        Ray ray;
	        RaycastHit hit;
	        
	        ray = Camera.main.ScreenPointToRay(screenPos);
	        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
	        {
	            // We got a direct hit on the target.
	            if (hit.collider.transform == target)
	                return true;

	            // Check all colliders within a sphere.
	            var colliders = Physics.OverlapSphere(hit.point, radius);
	            foreach (var c in colliders)
	            {
	                if (c.transform == target)
	                {
	                    return true;
	                }
	            }
	        }
	        
	        return false;
	    }

        public static void SendMessageDownwards(this Transform t, string methodName, SendMessageOptions options)
        {
            t.SendMessage(methodName, options);

            foreach (Transform child in t)
            {
                child.SendMessageDownwards(methodName, options);
            }
        }

        public static void SendMessageDownwards(this Transform t, string methodName, object value, SendMessageOptions options)
        {
            t.SendMessage(methodName, value, options);

            foreach (Transform child in t)
            {
                child.SendMessageDownwards(methodName, value, options);
            }
        }

        #region Positioning

        /// <summary>
        /// Orientates the Transform so that a reference position is directly below it, and perpendicular to the transform's forward vector.
        /// </summary>
        /// <remarks>
        /// This is useful for positioning objects around a sphere surface.
        /// </remarks>
        /// <param name="tx">The Transform to orientate.</param>
        /// <param name="referencePos">The position will orientated to.</param>
        public static void OrientateAbove(this Transform tx, Vector3 referencePos)
        {
            tx.LookAt(referencePos);
            tx.Rotate(Vector3.right, -90f);
        }

        /// <summary>
        /// Sets the Transform's position so that it is at the intersection with a layer directly below the Transform's downwards vector.
        /// </summary>
        /// <remarks>
        /// This is useful for snapping an object down onto a terrain (provided that it is correctly oriented with the terrain).
        /// </remarks>
        /// <param name="tx">The Transform to reposition.</param>
        /// <param name="layerMask">The layer mask being snapped to.</param>
        /// <param name="raiseRaycast">The distance above the transform to use as the origin of the raycast.</param>
        /// <param name="raiseSnap">The distance above the raycast hit point to move the snapped position.</param>
        public static void SnapDown(this Transform tx, int layerMask, float raiseRaycast = 0f, float raiseSnapped = 0f)
        {
            var config = RaycastConfig.Ray(layerMask: layerMask);
            tx.SnapDown(config, raiseRaycast, raiseSnapped);
        }

        /// <summary>
        /// Sets the Transform's position so that it is at the intersection with a layer directly below the Transform's downwards vector.
        /// </summary>
        /// <remarks>
        /// This is useful for snapping an object down onto a terrain (provided that it is correctly oriented with the terrain).
        /// </remarks>
        /// <param name="tx">The Transform to reposition.</param>
        /// <param name="config">The raycast configuration.</param>
        /// <param name="raiseRaycast">The distance above the transform to use as the origin of the raycast.</param>
        /// <param name="raiseSnap">The distance above the raycast hit point to move the snapped position.</param>
        public static void SnapDown(this Transform tx, RaycastConfig config, float raiseRaycast = 0f, float raiseSnapped = 0f)
        {
            var p = tx.position + tx.up * raiseRaycast;
            var snapped = p.SnapToLayer(-tx.up, config) + tx.up * raiseSnapped;
            tx.position = snapped;
        }

        #endregion
	}
}