namespace Andoco.Unity.Framework.Core.Scene.Management
{
    using UnityEngine;

    public static class GameObjectManagerExtensions
    {
        public static GameObject GetOrDefault(this IGameObjectManager goMgr, string kind, string label)
        {
            GameObject instance;
            if (goMgr.TryGet(kind, label, out instance))
            {
                return instance;
            }

            return null;
        }

		public static GameObject Create(this IGameObjectManager goMgr, GameObject prefab)
		{
			return goMgr.Create(prefab, Vector3.zero, Quaternion.identity);
		}

		public static GameObject Create(this IGameObjectManager goMgr, GameObject prefab, Vector3 position)
		{
			return goMgr.Create(prefab, position, Quaternion.identity);
		}
    }
}
