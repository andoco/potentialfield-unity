using System.Collections.Generic;
using Andoco.Core.Collections;

namespace Andoco.Unity.Framework.PotentialField
{
	public sealed class PotentialCache
	{
		readonly MultiMap<int, Entry> potentialCache = new MultiMap<int, Entry>();
		readonly float lifetime;

		public PotentialCache(float lifetime)
		{
			this.lifetime = lifetime;
		}

		public bool TryGet(int nodeIdx, int potentialLayerMask, PotentialFilter filter, float currentTime, out float potential)
		{
			var hashcode = Entry.GetHashCode(nodeIdx, potentialLayerMask, filter);

			potential = 0f;
			var hit = false;
			IList<Entry> cachedSamples;

			if (potentialCache.TryGet(hashcode, out cachedSamples))
			{
				for (int i = 0; i < cachedSamples.Count; i++)
				{
					var cs = cachedSamples[i];

					if (cs.node == nodeIdx && cs.layerMask == potentialLayerMask && cs.filter == filter)
					{
						if (currentTime - cs.time >= lifetime)
							break;

						potential = cs.potential;
						hit = true;
						break;
					}
				}
			}

			return hit;
		}

		public void Set(int nodeIdx, int potentialLayerMask, PotentialFilter filter, float currentTime, float potential)
		{
			var hashcode = Entry.GetHashCode(nodeIdx, potentialLayerMask, filter);

			var item = new Entry(nodeIdx, potentialLayerMask, filter, currentTime, potential);

			potentialCache.Remove(hashcode, item);
			potentialCache.Add(hashcode, item);
		}

		private struct Entry
		{
			public readonly int node;
			public readonly int layerMask;
			public readonly PotentialFilter filter;
			public readonly float time;
			public readonly float potential;

			public Entry(int node, int layerMask, PotentialFilter filter, float time, float potential)
			{
				this.node = node;
				this.layerMask = layerMask;
				this.filter = filter;
				this.time = time;
				this.potential = potential;
			}

			public override bool Equals(object obj)
			{
				if (obj is Entry)
				{
					Entry e = (Entry)obj;

					return this.node == e.node && this.layerMask == e.layerMask && this.filter == e.filter;
				}

				return false;
			}

			public override int GetHashCode()
			{
				return Entry.GetHashCode(this.node, this.layerMask, this.filter);
			}

			public static int GetHashCode(int nodeIdx, int potentialLayerMask, PotentialFilter filter)
			{
				var hash = 17;
				hash = hash * 23 + nodeIdx.GetHashCode();
				hash = hash * 23 + potentialLayerMask.GetHashCode();

				if (filter != null)
					hash = hash * 23 + filter.GetHashCode();

				return hash;
			}
		}
	}
}
