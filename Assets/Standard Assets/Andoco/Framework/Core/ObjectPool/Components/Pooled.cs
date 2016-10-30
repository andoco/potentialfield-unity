namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;
    using UnityEngine.Events;

    public class Pooled : MonoBehaviour
    {
        [ReadOnly]
        public bool isPooled;

        [ReadOnly]
        public int spawnCount;

        public bool invokeSpawnedEvent;

        public UnityEvent spawnedEvent;

        public void Spawned()
        {
            if (this.invokeSpawnedEvent)
                this.spawnedEvent.Invoke();
        }
    }
}