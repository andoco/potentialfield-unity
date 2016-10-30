namespace Andoco.Unity.Framework.Data
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameData : MonoBehaviour, IGameData
    {
        private Dictionary<string, object> data;

        void Awake()
        {
            this.data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            DontDestroyOnLoad(this.gameObject);
        }

        public void Set<T>(string key, T data)
        {
            this.data[key] = data;
        }

        public T Get<T>(string key)
        {
            return (T)this.data[key];
        }

        public T GetOrAdd<T>(string key) where T : class, new()
        {
            object result;

            if (!this.data.TryGetValue(key, out result))
            {
                result = Activator.CreateInstance<T>();
                this.data.Add(key, result);
            }

            return (T)result;
        }

        public bool Has(string key)
        {
            return this.data.ContainsKey(key);
        }
    }
}
