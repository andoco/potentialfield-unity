using System.Collections.Generic;
using Andoco.Unity.Framework.Core;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialFieldSource : MonoBehaviour, IPotentialSource
    {
        private IList<PotentialFieldNodeSource> sources = new List<PotentialFieldNodeSource>();
        private IPotentialFieldSystem system;

        #region Lifecycle

        void Start()
        {
            this.Spawned();
        }

        void Spawned()
        {
            if (this.system == null)
            {
                this.system = Indexed.GetSingle<IPotentialFieldSystem>();
            }
        }

        void Recycled()
        {
            for (int i = 0; i < this.sources.Count; i++)
            {
                this.system.RemoveNodeSource(this.sources[i]);
            }
        }

        void OnDestroy()
        {
            this.Recycled();
        }

        #endregion

        #region Public methods

        public PotentialFieldNodeSource AddNodeSource(string sourceKey, int layers, float potential = 0f)
        {
            var source = this.system.AddNodeSource(this.gameObject, sourceKey, layers, potential);

            this.sources.Add(source);

            return source;
        }

        public void RemoveNodeSource(string sourceKey)
        {
            for (int i = 0; i < this.sources.Count; i++)
            {
                if (string.Equals(sourceKey, this.sources[i].SourceKey, System.StringComparison.OrdinalIgnoreCase))
                {
                    this.system.RemoveNodeSource(this.sources[i]);
                    this.sources.RemoveAt(i);
                    return;
                }
            }
        }

        public IList<PotentialFieldNodeSource> GetNodeSources()
        {
            return this.sources;
        }

        public PotentialFieldNodeSource GetNodeSource(string sourceKey)
        {
            for (int i = 0; i < this.sources.Count; i++)
            {
                if (string.Equals(sourceKey, this.sources[i].SourceKey, System.StringComparison.OrdinalIgnoreCase))
                {
                    return this.sources[i];
                }
            }

            return null;
        }

        #endregion
    }
}
