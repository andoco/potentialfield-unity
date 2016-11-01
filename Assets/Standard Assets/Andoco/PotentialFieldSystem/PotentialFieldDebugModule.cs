using System.Collections.Generic;
using Andoco.Core.Pooling;
using Andoco.Unity.Framework.Core;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialFieldDebugModule : MonoBehaviour
    {
        private PotentialFieldSystem potentialField;
        private List<IFieldNodeRef> nodes = new List<IFieldNodeRef>();
        private ParticleSystem debugParticleSystem;
        private ParticleSystem.Particle[] debugParticles;
        private float highestPotential;

        public float gizmoRadius = 1f;
        public Color neutral = Color.clear;
        public Color positive1 = new Color(1f, 0f, 0f);
        public Color positive2 = new Color(1f, 1f, 0f);
        public Color negative1 = new Color(0f, 0f, 1f);
        public Color negative2 = new Color(0f, 0.956862745f, 1f);

        public bool debugEnabled;
        public PotentialLayerMask debugLayerMask;
        public float debugPotentialScale = 1;
        public bool debugPotentialAutoScale = true;
        public Material debugParticleMaterial;
        public float debugParticleSize = 1;
        public ParticleSystemRenderMode debugParticleRenderMode;
        public DebugPositionMode debugPositionMode;
        public Vector3 debugPositionOffset;
        public float debugPositionScale = 1f;

        public void Init(PotentialFieldSystem potentialField)
        {
            this.potentialField = potentialField;

            this.nodes = new List<IFieldNodeRef>(this.potentialField.NumNodes);

            for (int i = 0; i < this.potentialField.NumNodes; i++)
            {
                this.nodes.Add(this.potentialField[i]);
            }
        }

        public void ToggleDebug()
        {
            this.debugEnabled = !this.debugEnabled;
        }

        void Update()
        {
            if (this.potentialField.IsReady)
            {
                if (this.debugEnabled)
                {
                    if (this.debugParticleSystem == null)
                    {
                        this.BuildDebugParticleSystem();
                    }

                    var sampleRequest = new SampleRequest
                    {
                        nodes = this.nodes,
                        potentialLayerMask = this.debugLayerMask.value
                    };

                    var potentials = this.potentialField.SamplePotential(sampleRequest);

                    if (this.debugPotentialAutoScale)
                    {
                        this.highestPotential = 0f;

                        for (int i = 0; i < this.nodes.Count; i++)
                        {
                            var potential = potentials[i];

                            this.highestPotential = Mathf.Max(this.highestPotential, Mathf.Abs(potential));
                        }

                        this.debugPotentialScale = this.highestPotential > 0f ? 1f / this.highestPotential : 1;
                    }

                    for (int i = 0; i < this.nodes.Count; i++)
                    {
                        var potential = potentials[i];
                        var normPotential = Mathf.Clamp(this.debugPotentialScale * potential, -1f, 1f);

                        debugParticles[i].startColor = ColorHelper.HeatMapColor(normPotential, this.neutral, this.positive1, this.positive2, this.negative1, this.negative2);
                    }

                    potentials.ReturnToPool();

                    debugParticleSystem.SetParticles(debugParticles, debugParticles.Length);
                }
                else
                {
                    if (this.debugParticleSystem != null)
                    {
                        Destroy(this.debugParticleSystem);
                        this.debugParticleSystem = null;
                        this.debugParticles = null;
                    }
                }
            }
        }

        private void BuildDebugParticleSystem()
        {
            debugParticleSystem = this.gameObject.AddComponent<ParticleSystem>();
            debugParticleSystem.maxParticles = int.MaxValue;
            debugParticleSystem.startSize = this.debugParticleSize;
            debugParticleSystem.startLifetime = float.MaxValue;

            var particleRenderer = debugParticleSystem.GetComponent<ParticleSystemRenderer>();
            particleRenderer.material = this.debugParticleMaterial;
            particleRenderer.renderMode = this.debugParticleRenderMode;

            var shape = debugParticleSystem.shape;
            shape.enabled = false;
            var emission = debugParticleSystem.emission;
            emission.enabled = false;

            debugParticles = new ParticleSystem.Particle[this.nodes.Count];

            for (int i = 0; i < this.nodes.Count; i++)
            {
                Vector3 pos;
                switch (this.debugPositionMode)
                {
                    case DebugPositionMode.None:
                        pos = this.nodes[i].Position;
                        break;
                    case DebugPositionMode.Offset:
                        pos = this.nodes[i].Position + debugPositionOffset;
                        break;
                    case DebugPositionMode.ScaleOut:
                        pos = this.nodes[i].Position * this.debugPositionScale;
                        break;
                    default:
                        throw new System.InvalidOperationException(string.Format("Unknown debug position mode {0}", this.debugPositionMode));
                }

                debugParticles[i] = new ParticleSystem.Particle();
                debugParticles[i].position = pos;
                debugParticles[i].startSize = this.debugParticleSize;
                debugParticles[i].startLifetime = float.MaxValue;
            }

            debugParticleSystem.SetParticles(debugParticles, debugParticles.Length);

            debugParticleSystem.Play();
        }

        public enum DebugPositionMode
        {
            None,
            Offset,
            ScaleOut
        }
    }
}
