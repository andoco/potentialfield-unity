namespace Andoco.Unity.Framework.Core
{
	using System;
	using UnityEngine;

    public enum RaycastMode
    {
        Ray,
        Sphere
    }

    public struct RaycastConfig
    {
        public RaycastMode mode;
        public float maxDistance;
        public int layerMask;
        public float radius;
        public float raiseRaycast;

        public static RaycastConfig Ray(float radius = 1f, float maxDistance = Mathf.Infinity, int layerMask = 0, float raiseRaycast = 0f)
        {
            return new RaycastConfig { mode = RaycastMode.Ray, maxDistance = maxDistance, layerMask = layerMask, raiseRaycast = raiseRaycast };
        }

        public static RaycastConfig Sphere(float radius = 1f, float maxDistance = Mathf.Infinity, int layerMask = 0, float raiseRaycast = 0f)
        {
            return new RaycastConfig { mode = RaycastMode.Sphere, radius = radius, maxDistance = maxDistance, layerMask = layerMask, raiseRaycast = raiseRaycast };
        }
    }

	public static class RayExtensions
	{
		public static Vector3? GetHitPoint(this Ray ray)
		{
			return ray.GetHitPoint(LayerMaskConstants.LayerMaskAll);
		}

		public static Vector3? GetHitPoint(this Ray ray, int layerMask)
		{
			RaycastHit hit;
			Vector3? p = null;

			if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
			{
				p = hit.point;
			}
			
			return p;
		}

        public static bool TryGetHitPoint(this Ray ray, out Vector3 hitPoint, int layerMask = LayerMaskConstants.LayerMaskAll)
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                hitPoint = hit.point;
                return true;
            }

            hitPoint = Vector3.zero;

            return false;
        }

        /// <summary>
        /// Performs a raycast according to the supplied configuration.
        /// </summary>
        /// <returns><c>true</c> if a collider was hit, otherwise <c>false</c>.</returns>
        public static bool TryCast(this Ray ray, out RaycastHit hit, RaycastConfig config)
        {
            // Raise ray origin to avoid missing hits in close proximity.
            if (config.raiseRaycast > 0f)
                ray.origin += -ray.direction * config.raiseRaycast;
            
            switch (config.mode)
            {
                case RaycastMode.Ray:
                    if (Physics.Raycast(ray, out hit, config.maxDistance, config.layerMask))
                    {
                        return true;
                    }
                    break;
                case RaycastMode.Sphere:
                    if (Physics.SphereCast(ray, config.radius, out hit, config.maxDistance, config.layerMask))
                    {
                        return true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("config", config.mode, "Unknown RaycastMode");
            }

            return false;
        }
	}
}