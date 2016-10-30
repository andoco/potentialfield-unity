namespace Andoco.Unity.Framework.Core.ObjectPooling.Creator
{
	using UnityEngine;
    using Zenject;

    public class ZenjectGameObjectCreator : IGameObjectCreator
    {
        private IInstantiator instantiator;

        public ZenjectGameObjectCreator(IInstantiator instantiator)
        {
            this.instantiator = instantiator;
        }

        public GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            var go = this.instantiator.InstantiatePrefab(prefab);
            go.transform.position = position;
            go.transform.rotation = rotation;
            
            return go;
        }
    }
}