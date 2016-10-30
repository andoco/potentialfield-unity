namespace Andoco.Unity.Framework.Core.ObjectPooling.Creator
{
	using UnityEngine;

    public class DefaultGameObjectCreator : IGameObjectCreator
    {
        public GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return GameObject.Instantiate(prefab, position, rotation) as GameObject;
        }
    }

}