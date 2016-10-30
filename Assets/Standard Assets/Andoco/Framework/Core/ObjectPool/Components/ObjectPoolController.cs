namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;
    using Zenject;

	public class ObjectPoolController : MonoBehaviour
	{		
        public bool poolEnabled = true;
        public AutoPoolInfo[] autoPoolPrefabs;

        public GameObjectEvent spawned;
        public GameObjectEvent recycled;

        [Inject]
        public IObjectPool Pool { get; private set; }

        [Inject]
        void OnPostInject()
        {
            this.Pool.ObjectSpawned += this.OnSpawned;
            this.Pool.ObjectRecycled += this.OnRecycled;

            this.Pool.PoolEnabled = this.poolEnabled;

            this.Pool.AutoPool(this.autoPoolPrefabs);
        }

		void LateUpdate()
		{
            this.Pool.Update();
		}

        void OnDestroy()
        {
            this.Pool.ObjectSpawned -= this.OnSpawned;
            this.Pool.ObjectRecycled -= this.OnRecycled;
        }

        private void OnSpawned(Transform obj, GameObject prefab, bool isNewInstance)
        {
            this.spawned.Invoke(obj.gameObject);
        }

        private void OnRecycled(Transform obj, GameObject prefab, bool isPooled)
        {
            this.recycled.Invoke(obj.gameObject);
        }
	}
}