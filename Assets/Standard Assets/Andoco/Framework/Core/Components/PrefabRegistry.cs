namespace Andoco.Unity.Framework.Core
{
	using System.Collections.Generic;
	using System.Linq;
	using UnityEngine;
    using Zenject;

	[System.Serializable]
	public class PrefabRegistryItem
	{
		public PrefabRegistryItem(string key, GameObject go)
		{
			this.Key = key;
			this.Value = go;
		}

		public string Key;
		public GameObject Value;
	}

	public class PrefabRegistry : MonoBehaviour
	{
        [Inject]
        private IPrefabRegistry registry;
		
		public List<PrefabRegistryItem> Prefabs = new List<PrefabRegistryItem>();

        [Inject]
        void OnPostInject()
        {
            for (int i = 0; i < this.Prefabs.Count; i++)
            {
                this.registry.RegisterPrefab(this.Prefabs[i].Value, this.Prefabs[i].Key);
            }
        }
	}

    public interface IPrefabRegistry
    {
        void RegisterPrefab(GameObject prefab, string name = null);

        GameObject FindByName(string name);
    }

    public class DefaultPrefabRegistry : IPrefabRegistry
    {
        private List<PrefabRegistryItem> Prefabs = new List<PrefabRegistryItem>();

        public void RegisterPrefab(GameObject prefab, string name = null)
        {
            name = name ?? prefab.name;
            var item = this.Prefabs.SingleOrDefault(x => x.Key == name);
            if (item == null)
                this.Prefabs.Add(new PrefabRegistryItem(name, prefab));
            else
                item.Value = prefab;
        }

        public GameObject FindByName(string name)
        {
            var item = this.Prefabs.SingleOrDefault(x => string.Equals(x.Key, name, System.StringComparison.OrdinalIgnoreCase));

            return item == null ? null : item.Value;
        }
    }
}