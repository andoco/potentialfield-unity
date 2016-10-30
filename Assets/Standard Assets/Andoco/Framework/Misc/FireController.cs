namespace Andoco.Unity.Framework.Misc
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Sensors;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public class FireController : MonoBehaviour
    {
        private const string LogNumDetectedKey = "numDetected";

        private IList<GameObject> detected = new List<GameObject>();
        private Spawner spawner;
        private bool ceasefireFlagged;
        private Coroutine fireRoutine;

        [Inject]
        private IStructuredLog log;

        [SerializeField]
        [ReadOnly]
        private bool isFiring;

        [SerializeField]
        [ReadOnly]
        private bool isLocked;

        public GameObjectGroup Groups;

        [Tooltip("Fires once, and then enters a locked state until manually unlocked.")]
        public bool lockAfterFired;

        public bool IsFiring { get { return this.isFiring; } }

        void Awake()
        {
            this.spawner = this.GetComponent<Spawner>();

            if (this.Groups == null)
                this.Groups = this.GetComponent<GameObjectGroup>();
        }

        void OnDrawGizmos()
        {
            var pos = this.transform.position;
            Gizmos.DrawLine(pos, pos + this.transform.up * 50f);
        }

        void Recycled()
        {
            this.isFiring = false;
            this.isLocked = false;
            this.ceasefireFlagged = false;
            this.detected.Clear();
            this.fireRoutine = null;
        }

        public void FillProjectileTargets(IList<Transform> collector, Vector3 pos, Vector3 up)
        {
            for (int i = 0; i < this.detected.Count; i++)
            {
                collector.Add(this.detected[i].transform);
            }
        }

        public void Ceasefire()
        {
            if (this.fireRoutine != null)
            {
                this.ceasefireFlagged = true;
            }
        }

        public void Fire(string group)
        {
            Assert.IsFalse(string.IsNullOrEmpty(group));

            if (this.isLocked)
                return;
            
            this.fireRoutine = StartCoroutine(this.FireRoutine(group));
        }

        public void Unlock()
        {
            this.isLocked = false;
        }

        /// <summary>
        /// Receives UnityEvent callbacks with the transform of the fired object.
        /// </summary>
        public void OnPostFire(Transform firedTx)
        {
            if (this.detected.Count > 0 && ObjectValidator.Validate(this.detected[0]))
                firedTx.GetComponent<IAimable>().Aim(this.detected[0].transform);
        }

        private IEnumerator FireRoutine(string group)
        {
            this.isFiring = true;

            this.log.Info("Firing", x => x.Field(LogNumDetectedKey, this.detected.Count));

            this.detected.Clear();
            this.Groups.Fill(this.detected, group);

            if (this.detected.Count > 0)
            {
                // Keep spawning until all targets are destroyed/removed.
                while (this.detected.Count > 0 && !this.ceasefireFlagged)
                {
                    if (!this.spawner.IsSpawning)
                    {
                        this.detected.Clear();
                        this.Groups.Fill(this.detected, group);

                        this.spawner.Spawn();

                        if (this.lockAfterFired)
                        {
                            this.isLocked = true;
                            break;
                        }
                    }

                    yield return null;
                }

                while (this.spawner.IsSpawning && !this.ceasefireFlagged)
                {
                    yield return null;
                }
            }

            this.isFiring = false;
            this.fireRoutine = null;
            this.ceasefireFlagged = false;
            this.detected.Clear();
        }
    }
}
