namespace Andoco.Unity.Framework.Weighting
{
    using System.Collections.Generic;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;

    public static class WeightingExtensions
    {
		public static WeightedItem WeightByDistance(this WeightedItem item, Vector3 origin, float maxDist, float scale)
		{
			if (maxDist <= 0f)
				throw new System.ArgumentOutOfRangeException("maxDist", maxDist, "maxDist must be greater than zero");
			
			var dist = Vector3.Distance(origin, item.Target.TargetPosition);
			var distWeight = scale * (1f - (1f / maxDist * Mathf.Min(dist, maxDist)));
			
			var newItem = new WeightedItem(item);
			newItem.Weight += distWeight;
			
			return newItem;
		}

        public static IEnumerable<WeightedItem> WeightByDistance(this IEnumerable<WeightedItem> items, Vector3 origin, float maxDist, float scale)
        {
            if (maxDist <= 0f)
                throw new System.ArgumentOutOfRangeException("maxDist", maxDist, "maxDist must be greater than zero");
            
            foreach (var item in items)
            {                
                yield return item.WeightByDistance(origin, maxDist, scale);
            }
        }

		public static WeightedItem WeightByInterpolation(this WeightedItem item, Vector3 origin, float maxDist, float scale, Interpolate.EaseType easeType)
		{
			var easeFunc = Interpolate.Ease(easeType);
			var start = 0f;
			var dist = scale;
			var duration = maxDist;
			
			var elapsed = Vector3.Distance(origin, item.Target.TargetPosition);
			var distWeight = scale - easeFunc(start, dist, elapsed, duration);
			
			var newItem = new WeightedItem(item);
			newItem.Weight += distWeight;

			return newItem;
		}
        
        public static IEnumerable<WeightedItem> WeightByInterpolation(this IEnumerable<WeightedItem> items, Vector3 origin, float maxDist, float scale, Interpolate.EaseType easeType)
        {
            var easeFunc = Interpolate.Ease(easeType);
            var start = 0f;
            var dist = scale;
            var duration = maxDist;
            
            foreach (var item in items)
            {
                var elapsed = Vector3.Distance(origin, item.Target.TargetPosition);
                var distWeight = scale - easeFunc(start, dist, elapsed, duration);
                
                var newItem = new WeightedItem(item);
                newItem.Weight += distWeight;
                
                yield return newItem;
            }
        }
    }
}