namespace Andoco.Unity.Framework.Core.Scene.Management
{
    using UnityEngine;
    using UnityEngine.Assertions;

    public class GameObjectManager : IGameObjectManager
    {
        private readonly IGameObjectStore gameObjectStore;
        private readonly IPrefabSelector prefabSelector;
        private readonly IObjectPool objectPool;

        public GameObjectManager(IGameObjectStore anchorStore, IPrefabSelector prefabSelector, IObjectPool objectPool)
        {
            this.gameObjectStore = anchorStore;
            this.prefabSelector = prefabSelector;
            this.objectPool = objectPool;
        }

        public GameObject Create(string kind, string label = null)
        {
            Assert.IsFalse(string.IsNullOrEmpty(kind));

            var prefab = this.prefabSelector.Select(kind);

            Assert.IsNotNull(prefab, string.Format("Prefab not found for GameObject kind {0}", kind));

            return this.Create(prefab, Vector3.zero, Quaternion.identity, label);
        }

        public GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation, string label = null)
        {
            var go = this.objectPool.Spawn(prefab, position, rotation).gameObject;

            var entity = go.GetComponent<Entity>();

            if (entity == null)
            {
                entity = go.AddComponent<Entity>();
            }

            entity.kind = prefab.name;
            entity.label = label;

            this.gameObjectStore.Add(go, label);

            return go;
        }

        public void Destroy(GameObject gameObject)
        {
            Assert.IsNotNull(gameObject);

            this.gameObjectStore.Remove(gameObject);

            this.objectPool.RecycleOrDestroy(gameObject.transform);
        }

        public GameObject GetOrCreate(string kind, string label = null)
        {
            Assert.IsFalse(string.IsNullOrEmpty(kind));

            var prefab = this.prefabSelector.Select(kind);

            Assert.IsNotNull(prefab, string.Format("Prefab not found for GameObject kind {0}", kind));

            return this.GetOrCreate(prefab, label);
        }

        public GameObject GetOrCreate(GameObject prefab, string label = null)
        {
            Assert.IsNotNull(prefab);

            GameObject go = null;

            if (!string.IsNullOrEmpty(label))
            {
                // Check for stored gameobject with matching label.
                if (!this.gameObjectStore.TryGetByLabel(prefab, label, out go))
                {
                    // Stored gameobject not found so try and find in scene.
                    var entities = GameObject.FindObjectsOfType<Entity>();

                    foreach (var entity in entities)
                    {
                        if (
                            string.Compare(prefab.name, entity.kind, System.StringComparison.OrdinalIgnoreCase) == 0 &&
                            string.Compare(label, entity.label, System.StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            go = entity.gameObject;
                        }
                    }
                }
            }

            // If no gameobject stored or found in scene, make a new one.
            if (go == null)
            {
                go = this.Create(prefab, Vector3.zero, Quaternion.identity, label);
            }

            return go;
        }

        public bool IsAlive(Object obj)
        {
            if (obj == null)
                return false;

            GameObject go;

            if (obj is GameObject)
                go = (GameObject)obj;
            else if (obj is Component)
                go = ((Component)obj).gameObject;
            else
                throw new System.ArgumentOutOfRangeException("obj", obj, "A GameObject must be accessible from the supplied type");

            return !this.objectPool.IsPooled(go);
        }

        public bool TryGet(string kind, string label, out GameObject go)
        {
            Assert.IsFalse(string.IsNullOrEmpty(kind));
            var prefab = this.prefabSelector.Select(kind);
            Assert.IsNotNull(prefab, string.Format("Prefab not found for GameObject kind {0}", kind));

            return this.TryGet(prefab, label, out go);
        }

        public bool TryGet(GameObject prefab, string label, out GameObject go)
        {
            if (this.gameObjectStore.TryGetByLabel(prefab, label, out go))
            {
                return true;
            }

            go = null;
            return false;
        }
    }
}
