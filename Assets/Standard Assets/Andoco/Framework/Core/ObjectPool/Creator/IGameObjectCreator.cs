namespace Andoco.Unity.Framework.Core.ObjectPooling.Creator
{
	using UnityEngine;
	using System.Collections.Generic;
	using System.Linq;

    public interface IGameObjectCreator
    {
        GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation);
    }
}