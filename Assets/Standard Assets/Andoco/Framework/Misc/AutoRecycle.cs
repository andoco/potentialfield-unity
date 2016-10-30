namespace Andoco.Unity.Framework.Misc
{
    using UnityEngine;
    using System.Collections;
    using Andoco.Unity.Framework.Core;
    using Zenject;

    public class AutoRecycle : MonoBehaviour
    {
        [Inject]
        private IObjectPool objectPool;

        [Tooltip("The time in seconds after which the gameobject will be recycled")]
        public float recycleAfter;

        [Tooltip("The random time variance to add to the base recycleAfter time")]
        public float recycleAfterVariance;

        void Start()
        {
            if (this.objectPool == null)
            {
                GameObject.Destroy(this.gameObject, this.GetRecycleDelay());
            }
            else
            {
                this.StartCoroutine(this.Run());
            }
        }

        void Spawned()
        {
            this.StartCoroutine(this.Run());
        }

        void Recycled()
        {
            this.StopAllCoroutines();
        }

        private float GetRecycleDelay()
        {
            return this.recycleAfter + Random.value * this.recycleAfterVariance;
        }

        private IEnumerator Run()
        {
            yield return new WaitForSeconds(this.GetRecycleDelay());
            this.objectPool.RecycleOrDestroy(this.transform);
        }
    }
}
