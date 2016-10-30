namespace Andoco.Unity.Framework.Core.Scene
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SceneOrganizerConfig
    {
        public bool autoNumberInstances = true;
        public bool organizeByPrefab = true;
        public Transform activeRoot;
        public Transform inactiveRoot;
        public bool removeEmptyPrefabRoots = true;
    }
}