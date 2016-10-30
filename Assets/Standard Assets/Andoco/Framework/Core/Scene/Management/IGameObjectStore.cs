namespace Andoco.Unity.Framework.Core.Scene.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public interface IGameObjectStore
    {
        void Add(GameObject go, string label);

        void Remove(GameObject go);

        bool TryGetByLabel(GameObject prefab, string label, out GameObject go);
    }
}
