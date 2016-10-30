namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;
	using System.Collections.Generic;

    public class DefaultPrefabSelector : IPrefabSelector
    {
        public IDictionary<string, GameObject> Prefabs { get; private set; }
        
        public DefaultPrefabSelector()
        {
            this.Prefabs = new Dictionary<string, GameObject>();
        }
        
        public GameObject Select(string name)
        {
            return this.Prefabs[name];
        }
    }
}