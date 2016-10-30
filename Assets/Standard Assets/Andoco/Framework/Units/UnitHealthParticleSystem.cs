using Andoco.Unity.Framework.Core.Scene.Management;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.Units
{
    public class UnitHealthParticleSystem : MonoBehaviour
    {
        [Inject]
        private IGameObjectManager goMgr;

        private GameObject particleSysGo;
        private ParticleSystem particleSys;

        public Unit unit;
        public GameObject particleSystemPrefab;
        public HealthLevelConfig[] healthLevels;

        void Start()
        {
            if (this.unit == null)
            {
                this.unit = this.GetComponent<Unit>();
            }
        }

        void Update()
        {
//            var healthPerc = 1f - (1f / (this.unit.health.Max - this.unit.health.Min) * (this.unit.health.Value - this.unit.health.Min));
            var healthPerc = (this.unit.health.Value - this.unit.health.Min) / (this.unit.health.Max - this.unit.health.Min);

            for (int i = 0; i < this.healthLevels.Length; i++)
            {
                var conf = this.healthLevels[i];

                if (healthPerc < conf.threshold)
                {
                    var ps = this.GetOrCreateParticleSystem();
                    var emission = ps.emission;
                    emission.rate = conf.emissionRate;
                }
            }
        }

        void Recycled()
        {
            this.goMgr.Destroy(this.particleSys.gameObject);
        }

        void OnDestroy()
        {
            if (this.particleSys != null)
            {
                this.goMgr.Destroy(this.particleSys.gameObject);
            }
        }

        private ParticleSystem GetOrCreateParticleSystem()
        {
            if (this.particleSys == null)
            {
                this.particleSysGo = this.goMgr.Create(this.particleSystemPrefab);
                this.particleSysGo.transform.parent = this.unit.transform;
                this.particleSysGo.transform.localPosition = Vector3.zero;
                this.particleSysGo.transform.localRotation = Quaternion.identity;
                this.particleSys = this.particleSysGo.GetComponentInChildren<ParticleSystem>();
            }

            return this.particleSys;
        }

        [System.Serializable]
        public class HealthLevelConfig
        {
            [Range(0f, 1f)]
            public float threshold;

            public float emissionRate;
        }
    }
}
