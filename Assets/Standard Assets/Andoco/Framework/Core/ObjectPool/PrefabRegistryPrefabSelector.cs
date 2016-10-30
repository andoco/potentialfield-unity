namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;
	using System.Collections.Generic;

    public class PrefabRegistryPrefabSelector : IPrefabSelector
    {
        private readonly IPrefabRegistry prefabRegistry;

        public PrefabRegistryPrefabSelector(IPrefabRegistry prefabRegistry)
        {
            this.prefabRegistry = prefabRegistry;
        }

        public GameObject Select(string name)
        {
            return this.prefabRegistry.FindByName(name);
        }
    }
}