namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.Unity.Framework.Core.ObjectPooling.Creator;

    public class ObjectPool : IObjectPool
    {
        #region Fields

        private readonly IDictionary<GameObject, PrefabInfo> prefabInfo = new Dictionary<GameObject, PrefabInfo>();

        private readonly Dictionary<Transform, GameObject> prefabLookup = new Dictionary<Transform, GameObject>();

        private readonly HashSet<Transform> scheduledForRecycle = new HashSet<Transform>();

        private readonly List<Transform> toRecycle = new List<Transform>();

        private bool isPrePooling;

        private bool isFirstUpdate = true;

        private readonly IGameObjectCreator creator;

        #endregion

        public ObjectPool(IGameObjectCreator creator)
        {
            this.creator = creator;
            this.PoolEnabled = true;
        }

        #region Events

        public event ObjectSpawnedDelegate ObjectSpawned;

        public event ObjectRecycledDelegate ObjectRecycled;

        #endregion

        public bool PoolEnabled { get; set; }

        #region Public methods

        public void AutoPool(AutoPoolInfo[] autoPoolPrefabs)
        {
            this.isPrePooling = true;

            foreach (var info in autoPoolPrefabs)
            {
                this.CreatePool(info.prefab, info.quantity);
            }

            this.isPrePooling = false;
        }

        public void CreatePool(GameObject prefab)
        {
            this.CreatePool(prefab, 0);
        }

        public void CreatePool(GameObject prefab, int initialSize)
        {
            if (!this.PoolEnabled)
                throw new System.InvalidOperationException(string.Format("Cannot create a pool for {0} because the pool is not enabled", prefab));

            Debug.Log(string.Format("Creating pool for {0} with initial size {1}", prefab, initialSize));

            if (this.prefabInfo.ContainsKey(prefab))
                throw new System.InvalidOperationException(string.Format("Cannot create a pool for {0} because the pool already exists", prefab));

            var info = this.GetOrCreatePrefabInfo(prefab);
            info.IsPooled = true;

            var spawnedObjects = new List<Transform>();

            // IMPORTANT: Must spawn all initial objects before any get recycled, otherwise
            // we'll end up reusing the same one every time.
            for (int i = 0; i < initialSize; i++)
            {
                var transform = Spawn(prefab);
                spawnedObjects.Add(transform);
            }

            // Recycle all initial objects in the pool.
            foreach (var transform in spawnedObjects)
            {
                // Using RecycleImmediate will cause the object to be recycled before its Start
                // method has been called. This will result in both Start and Spawned being called
                // at the same time when the object is next spawned, which could be problematic.
                //RecycleImmediate(transform);

                // Using Recycle instead of RecycleImmediate allow the Start methods of the object's 
                // components to be called before it is recycled. The next time it is spawned, only
                // the Spawned methods will be called.
                Recycle(transform);
            }
        }

        /// <summary>
        /// Spawn the specified prefab with a position and rotation.
        /// </summary>
        /// <remarks>
        /// IMPORTANT: Objects should not be spawned during the first frame because any pre-pooled
        /// instances will not have been recycled yet.
        /// </remarks>
        /// <param name="prefab">Prefab.</param>
        /// <param name="position">Position.</param>
        /// <param name="rotation">Rotation.</param>
        public Transform Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null)
                throw new System.ArgumentNullException("prefab", "Prefab must be supplied to spawn an instance");

            if (this.PoolEnabled)
            {
                if (this.isFirstUpdate && !this.isPrePooling)
                    Debug.LogWarning("Spawning an object before prepooling frame is complete");

                PrefabInfo info;

                // Get the prefab info.
                if (!this.prefabInfo.TryGetValue(prefab, out info))
                {
                    // Create prefab info for any non-pooled prefabs. This allows us to organise and track the instances
                    // even though they won't be returned to the pool.

                    info = new PrefabInfo
                    {
                        Prefab = prefab,
                        IsPooled = false
                    };

                    this.prefabInfo.Add(prefab, info);
                }

                Pooled pooled = null;
                Transform obj = null;
                var list = info.Instances;

                if (list.Count > 0)
                {
                    // Remove instance from pool until we have one that hasn't been destroyed.
                    while (obj == null && list.Count > 0)
                    {
                        obj = list[0];
                        list.RemoveAt(0);
                    }

                    if (obj != null)
                    {
                        // We now have an instance from the pool we can setup for spawning.
                        obj.transform.localPosition = position;
                        obj.transform.localRotation = rotation;
                        obj.gameObject.SetActive(true);
                        this.prefabLookup.Add(obj, prefab);

                        pooled = obj.GetComponent<Pooled>();
                        pooled.isPooled = false;
                        pooled.spawnCount++;

                        obj.SendMessageDownwards("Spawned", SendMessageOptions.DontRequireReceiver);

                        this.OnObjectSpawned(obj, info.Prefab, false);

                        return obj;
                    }
                }

                // No pooled instance was available so make a new one.
                pooled = this.CreateNewInstance(info, position, rotation);
                obj = pooled.transform;
                this.OnObjectSpawned(obj, info.Prefab, true);

                return obj;
            }
            else
            {
                // The pool is not enabled so we just instantiate normally.
                var obj = ((GameObject)Object.Instantiate(prefab, position, rotation)).transform;
                this.prefabLookup[obj] = prefab;
                this.OnObjectSpawned(obj, prefab, true);
                return obj;
            }
        }

        public Transform Spawn(GameObject prefab, Vector3 position)
        {
            return Spawn(prefab, position, Quaternion.identity);
        }

        public Transform Spawn(GameObject prefab)
        {
            return Spawn(prefab, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Schedule the object to be recycled during the LateUpdate stage of the current frame.
        /// </summary>
        /// <param name="obj">Object.</param>
        public void Recycle(Transform obj)
        {
            this.scheduledForRecycle.Add(obj);
        }

        public bool TryRecycle(Transform obj)
        {
            if (
                this.PoolEnabled &&
                this.prefabLookup.ContainsKey(obj))
            {
                this.Recycle(obj);
                return true;
            }

            return false;
        }

        public void RecycleOrDestroy(Transform obj)
        {
            if (!this.TryRecycle(obj))
            {
                GameObject.Destroy(obj.gameObject);
            }
        }

        public int Count(GameObject prefab)
        {
            if (this.prefabInfo.ContainsKey(prefab))
                return this.prefabInfo[prefab].Instances.Count;
            else
                return 0;
        }

        public int CountSpawned(GameObject prefab)
        {
            var count = 0;

            var enumerator = this.prefabLookup.Values.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == prefab)
                {
                    count++;
                }
            }

            enumerator.Dispose();

            return count;
        }

        /// <summary>
        /// Returns objects that have been spawned from instance, even if they do no belong to a pool.
        /// </summary>
        /// <returns>The spawned instances.</returns>
        /// <param name="prefab">The prefab to return spawned instance for.</param>
        public IEnumerable<Transform> FindSpawnedInstances(GameObject prefab)
        {
            return this.prefabLookup.Where(item => item.Value == prefab).Select(item => item.Key);
        }

        public IEnumerable<Transform> FindSpawnedInstances()
        {
            return this.prefabLookup.Keys;
        }

        public bool IsPooled(GameObject go)
        {
            if (go == null)
                return false;

            var pooled = go.GetComponent<Pooled>();

            return (pooled != null) && pooled.isPooled;
        }

        public void Update()
        {
            if (this.scheduledForRecycle.Count > 0)
            {
                // Need to copy to array so that nested calls to Recycle() are possible from an object's Recycled() callback.
                this.toRecycle.AddRange(this.scheduledForRecycle);
                this.scheduledForRecycle.Clear();

                for (int i = 0; i < this.toRecycle.Count; i++)
                {
                    RecycleImmediate(this.toRecycle[i]);
                }

                this.toRecycle.Clear();
            }

            if (this.isFirstUpdate)
                this.isFirstUpdate = false;
        }

        #endregion

        private Pooled CreateNewInstance(PrefabInfo info, Vector3 position, Quaternion rotation)
        {
            var obj = this.creator.Create(info.Prefab, position, rotation);
            this.prefabLookup.Add(obj.transform, info.Prefab);

            var pooled = obj.GetComponent<Pooled>();

            if (pooled == null)
            {
                pooled = obj.gameObject.AddComponent<Pooled>();
            }

            return pooled;
        }

        private PrefabInfo GetOrCreatePrefabInfo(GameObject prefab)
        {
            PrefabInfo info;
            if (this.prefabInfo.ContainsKey(prefab))
            {
                info = this.prefabInfo[prefab];
            }
            else
            {
                info = new PrefabInfo
                {
                    Prefab = prefab,
                };

                this.prefabInfo[prefab] = info;
            }

            return info;
        }

        /// <summary>
        /// Immediately recycle the object by disabling it and returning it to the pool.
        /// </summary>
        /// <remarks>
        /// If an object is recycled using this method immediately after spawning, the object's
        /// Start() methods will not be called.
        /// </remarks>
        /// <param name="obj">Object.</param>
        private void RecycleImmediate(Transform obj)
        {
            GameObject prefab;
            PrefabInfo info;

            if (obj == null)
                throw new System.ArgumentNullException(string.Format("Cannot recycle object because it is null or destroyed. {0}", obj));

            // Check if a pool exists for the prefab
            if (
                this.PoolEnabled &&
                this.prefabLookup.TryGetValue(obj, out prefab) &&
                this.prefabInfo.TryGetValue(prefab, out info))
            {
                // Stop tracking the recycled object.
                this.prefabLookup.Remove(obj);

                if (info.IsPooled)
                {
                    info.Instances.Add(obj);

                    var pooled = obj.GetComponent<Pooled>();
                    pooled.isPooled = true;

                    // notify and set to inactive
                    obj.SendMessageDownwards("Recycled", SendMessageOptions.DontRequireReceiver);
                    obj.SendMessageDownwards("Restore", prefab, SendMessageOptions.DontRequireReceiver);

                    obj.gameObject.SetActive(false);

                    this.OnObjectRecycled(obj, info.Prefab, true);
                }
                else
                {
                    // No pool is available for the object so destroy normally. We need to set the parent to null
                    // first as it isn't immediately removed from its parent otherwise.
                    obj.parent = null;
                    GameObject.Destroy(obj.gameObject);
                    this.OnObjectRecycled(obj, info.Prefab, false);
                }
            }
            else
            {
                throw new System.InvalidOperationException(string.Format("Cannot recycle an instance that was not spawned from this pool. {0}", obj));
            }
        }

        private void OnObjectSpawned(Transform obj, GameObject prefab, bool isNewInstance)
        {
            if (this.ObjectSpawned != null)
                this.ObjectSpawned(obj, prefab, isNewInstance);
        }

        private void OnObjectRecycled(Transform obj, GameObject prefab, bool isPooled)
        {
            if (this.ObjectRecycled != null)
                this.ObjectRecycled(obj, prefab, isPooled);
        }

        #region Nested classes

        private class PrefabInfo
        {
            public PrefabInfo()
            {
                this.Instances = new List<Transform>();
            }

            public GameObject Prefab { get; set; }

            public bool IsPooled { get; set; }

            public int InstanceCounter { get; set; }

            public IList<Transform> Instances { get; private set; }
        }

        #endregion
    }

}