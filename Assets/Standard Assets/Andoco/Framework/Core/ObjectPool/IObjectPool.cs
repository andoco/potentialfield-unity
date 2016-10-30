namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;
	using System.Collections.Generic;

    public delegate void ObjectSpawnedDelegate(Transform obj, GameObject prefab, bool isNewInstance);

    public delegate void ObjectRecycledDelegate(Transform obj, GameObject prefab, bool isPooled);

    public interface IObjectPool
    {
        event ObjectSpawnedDelegate ObjectSpawned;
        
        event ObjectRecycledDelegate ObjectRecycled;

        bool PoolEnabled { get; set; }

        void AutoPool(AutoPoolInfo[] autoPoolPrefabs);

        int Count(GameObject prefab);

        int CountSpawned(GameObject prefab);

        void CreatePool(GameObject prefab, int initialSize);

        IEnumerable<Transform> FindSpawnedInstances(GameObject prefab);

        IEnumerable<Transform> FindSpawnedInstances();

        bool IsPooled(GameObject go);

        Transform Spawn(GameObject prefab, Vector3 position, Quaternion rotation);

        bool TryRecycle(Transform obj);

        void RecycleOrDestroy(Transform obj);

        void Update();
    }
}