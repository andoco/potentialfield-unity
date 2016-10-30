using System.Collections;
using Andoco.Core.Diagnostics.Logging;
using Andoco.Unity.Framework.Core.ObjectPooling.Creator;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.Core
{
    public class Bootstrap : MonoBehaviour
    {
        [Inject]
        private IStructuredLog log;

        [Inject]
        private IGameObjectCreator creator;

        public BootstrapPrefab[] prefabs;

        [Inject]
        void OnPostInject()
        {
            StartCoroutine(this.InstantatePrefabsRoutine());
        }

        [System.Serializable]
        public class BootstrapPrefab
        {
            public GameObject prefab;
        }

        private IEnumerator InstantatePrefabsRoutine()
        {
            // Wait a frame to ensure all Zenject injection is finished.
            this.log.Trace("Waiting for end of frame").Write();
            yield return new WaitForEndOfFrame();

            this.log.Trace("Ready to instantiate prefabs in scene").Write();

            for (int i = 0; i < this.prefabs.Length; i++)
            {
                var prefab = this.prefabs[i].prefab;
                this.log.Trace("Creating new instance prefab").Field("prefab", prefab).Write();
                this.creator.Create(prefab, Vector3.zero, Quaternion.identity);
            }
        }
    }
}