namespace Andoco.Unity.Framework.Core.Scene
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Zenject;

    public class SceneOrganizer : MonoBehaviour, ISceneOrganizer
    {
        private const char AutoNumberPrefix = '-';
        private const string AutoNumberFormat = "{0}-{1:0000}";
        private const int AutoNumberLength = 5;
        private const int MaxInstances = 9999;

        private readonly Dictionary<GameObject, Info> info = new Dictionary<GameObject, Info>();

        [InjectOptional]
        private IObjectPool objectPool;

        public SceneOrganizerConfig config;

        [Inject]
        void OnPostInject()
        {
            if (this.objectPool != null)
            {
                this.objectPool.ObjectSpawned += this.OnObjectSpawned;
                this.objectPool.ObjectRecycled += this.OnObjectRecycled;
            }
        }

        #region Public methods
        
        public void Organize(Transform instance, GameObject prefab)
        {
            var info = this.GetOrCreateInfo(prefab);
            
            instance.parent = instance.gameObject.activeSelf ? info.Parent : info.InactiveParent;
            
            if (this.config.autoNumberInstances)
            {
                instance.name = AutoNumber(instance.name, info);
            }
        }

        public void Cleanup()
        {
            if (this.config.removeEmptyPrefabRoots)
            {
                var enumerator = this.info.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var info = enumerator.Current.Value;

//                    Debug.LogFormat("{0}: Cleaning scene for {1}", Time.time, info);

                    this.DestroyEmptyActiveParent(info);
                }
            }
        }

        private void DestroyEmptyActiveParent(Info info)
        {
            // There may be cases where parent is null due to the gameobject being manually
            // moved to another parent after spawning.

            if (
                info.Parent != null && 
                info.Parent != this.config.activeRoot && 
                info.Parent.childCount == 0)
            {
                GameObject.Destroy(info.Parent.gameObject);
            }
        }

        #endregion

        #region Private methods

        private void OnObjectSpawned(Transform obj, GameObject prefab, bool isNewInstance)
        {
            this.Organize(obj, prefab);
        }

        private void OnObjectRecycled(Transform obj, GameObject prefab, bool isPooled)
        {
            if (isPooled)
            {
                this.Organize(obj, prefab);
            }

            this.Cleanup();
        }
        
        private static string AutoNumber(string name, Info info)
        {
            if (!IsNumbered(name))
            {
                info.InstanceCounter++;
                
                if (info.InstanceCounter >= info.MaxInstances)
                {
                    throw new InvalidOperationException(string.Format("Cannot auto-number the instance name because the maximum number of instances ({0}) has been reached.", MaxInstances));
                }
                
                return string.Format(AutoNumberFormat, name, info.InstanceCounter);
            }
            
            return name;
        }
        
        private static bool IsNumbered(string name)
        {
            return name.Length > AutoNumberLength && name[name.Length - AutoNumberLength] == AutoNumberPrefix;
        }
        
        private Info GetOrCreateInfo(GameObject prefab)
        {
            Info info;
            if (!this.info.TryGetValue(prefab, out info))
            {
                info = new Info
                {
                    InactiveParent = this.GetParent(prefab, false),
                    Prefab = prefab,
                    MaxInstances = MaxInstances
                };
                
                this.info.Add(prefab, info);
            }

            if (info.Parent == null)
            {
                info.Parent = this.GetParent(prefab, true);
            }
            
            return info;
        }
        
        private Transform GetParent(GameObject prefab, bool isActive)
        {
            Transform parent;
            
            var root = isActive ? this.config.activeRoot : this.config.inactiveRoot;
            
            if (this.config.organizeByPrefab)
            {
                parent = root == null ? null : root.Find(prefab.name);
                
                if (parent == null)
                {
                    parent = new GameObject(prefab.name).transform;
                    parent.parent = root;
                }
            }
            else
            {
                parent = root;
            }
            
            return parent;
        }

        #endregion
        
        private sealed class Info
        {
            public Transform Parent { get; set; }
            
            public Transform InactiveParent { get; set; }
            
            public GameObject Prefab { get; set; }
            
            public int InstanceCounter { get; set; }

            public int MaxInstances { get; set; }

            public override string ToString()
            {
                return string.Format("[Info: Prefab={0}, Parent={1}, InactiveParent={2}, InstanceCounter={3}, MaxInstances={4}]", Prefab, Parent, InactiveParent, InstanceCounter, MaxInstances);
            }
        }
    }

}