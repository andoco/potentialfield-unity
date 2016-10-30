namespace Andoco.Unity.Framework.Core.Scene.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class DefaultGameObjectStore : IGameObjectStore
    {
        private readonly Dictionary<string, GameObject> labelledGameObjects = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<GameObject, string> labels = new Dictionary<GameObject, string>();
        private readonly List<GameObject> gameObjects = new List<GameObject>();

        public void Add(GameObject go, string label)
        {
            this.gameObjects.Add(go);

            if (!string.IsNullOrEmpty(label))
            {
                this.labelledGameObjects.Add(label, go);
                this.labels.Add(go, label);
            }
        }

        public void Remove(GameObject go)
        {
            this.gameObjects.Remove(go);

            string label;
            if (this.labels.TryGetValue(go, out label))
            {
                this.labels.Remove(go);
                this.labelledGameObjects.Remove(label);
            }
        }

        public bool TryGetByLabel(GameObject prefab, string label, out GameObject go)
        {
            return this.labelledGameObjects.TryGetValue(label, out go);
        }
    }
}
