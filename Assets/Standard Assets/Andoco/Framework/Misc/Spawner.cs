namespace Andoco.Unity.Framework.Misc
{
    using System;
    using System.Collections;
    using Andoco.BehaviorTree.Signals;
    using Andoco.Core;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Core.Scene.Management;
    using UnityEngine;
    using UnityEngine.Events;
    using Zenject;

    public class Spawner : MonoBehaviour
    {
        private const string preBatchSpawnLockFlagName = "preBatchSpawnLock";
        private const string preSpawnLockFlagName = "preSpawnLock";
        private const string postSpawnLockFlagName = "postSpawnLock";

        [Inject]
        private IGameObjectManager goMgr;

        [Inject]
        private FlagSignal flagSignal;

        private Coroutine spawnRoutine;

        public GameObject[] prefabs;
        public InterpolatedInt batchSize;
        public RandomInt batchSizeVariance;
        public InterpolatedFloat batchSpacingInterval;
        public RandomFloat batchSpacingIntervalVariance;
        public InterpolatedFloat batchInterval;
        public RandomFloat batchIntervalVariance;
        public bool limitTotalBatches;
        public InterpolatedInt totalBatches;
        public RandomInt totalBatchesVariance;
        public RandomFloat intervalBefore;
        public RandomFloat intervalAfter;

        [Tooltip("Starts the spawning routine automatically at startup")]
        public bool autoStart;

        [Tooltip("Skipping a number of frames allows time for other objects to initialize")]
        public int skipFrames = 1;

        [Tooltip("Spawns the first batch immediately, instead of waiting for the first duration")]
        public bool spawnImmediately;

        public bool destroyWhenFinished;
        public bool preBatchSpawnLock;
        public bool preSpawnLock;
        public bool postSpawnLock;

        public TransformEvent postSpawn;
        public UnityEvent postBatch;

        public float InterpolationFactor { get; set; }

        public bool IsSpawning { get; private set; }

        public int PendingCount { get; set; }

        public bool IsLocked { get; private set; }

        public Vector3 SpawnPosition { get; set; }

        public GameObject LastSpawned { get; private set; }

        [Inject]
        void OnPostInject()
        {
            if (this.autoStart)
            {
                this.Spawn();
            }
        }

        void Recycled()
        {
            this.IsSpawning = false;
        }

        public void Spawn()
        {
            if (this.IsSpawning)
                return;
            
            this.spawnRoutine = this.StartCoroutine(this.SpawnRoutine());
        }

        public void StopSpawning()
        {
            if (!this.IsSpawning)
                return;

            this.StopCoroutine(this.spawnRoutine);
        }

        public void Unlock()
        {
            this.IsLocked = false;
        }

        public IEnumerator SpawnRoutine()
        {
            this.IsSpawning = true;

            // Skip frames when starting to allow time for other objects to initialize.
            var pendingFrames = this.skipFrames;
            while (pendingFrames > 0)
            {
                pendingFrames--;
                yield return null;
            }

            if (this.intervalBefore.Min > 0f)
                yield return new WaitForSeconds(this.intervalBefore.Value);

            if (!this.spawnImmediately)
            {
                yield return new WaitForSeconds(this.batchInterval.GetValue(this.InterpolationFactor) + this.batchIntervalVariance.Value);
            }

            var maxBatches = this.totalBatches.GetValue(this.InterpolationFactor) + this.totalBatchesVariance.Value;
            var batchNum = 0;

            while (!this.limitTotalBatches || batchNum < maxBatches)
            {
                var batchSize = this.batchSize.GetValue(this.InterpolationFactor) + this.batchSizeVariance.Value;

                if (this.preBatchSpawnLock)
                {
                    this.PendingCount = batchSize;
                    yield return StartCoroutine(this.LockedRoutine(preBatchSpawnLockFlagName));
                    batchSize = this.PendingCount;
                }

                for (int j = 0; j < batchSize; j++)
                {
                    if (this.preSpawnLock)
                    {
                        this.PendingCount = 1;
                        yield return StartCoroutine(this.LockedRoutine(preSpawnLockFlagName));
                        this.PendingCount = 0;
                    }

                    var prefab = this.prefabs.PickRandom(UnityRandomNumber.Instance);
                    var go = this.goMgr.Create(prefab);
                    go.transform.position = this.SpawnPosition;
                    this.LastSpawned = go;

                    this.postSpawn.Invoke(go.transform);

                    if (this.postSpawnLock)
                    {
                        yield return StartCoroutine(this.LockedRoutine(postSpawnLockFlagName));
                    }

                    yield return new WaitForSeconds(this.batchSpacingInterval.GetValue(this.InterpolationFactor) + this.batchSpacingIntervalVariance.Value);
                }

                yield return new WaitForSeconds(this.batchInterval.GetValue(this.InterpolationFactor) + this.batchIntervalVariance.Value);

                batchNum++;
            }

            if (this.intervalAfter.Min > 0f)
                yield return new WaitForSeconds(this.intervalAfter.Value);

            this.IsSpawning = false;

            if (this.destroyWhenFinished)
                this.goMgr.Destroy(this.gameObject);
        }

        private IEnumerator LockedRoutine(string lockFlagName)
        {
            this.IsLocked = true;

            this.flagSignal.Dispatch(this.gameObject, new FlagSignal.Data(lockFlagName));

            while (this.IsLocked)
            {
                yield return null;
            }
        }
    }
}
