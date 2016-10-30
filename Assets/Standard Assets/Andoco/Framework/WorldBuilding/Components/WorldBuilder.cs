namespace Andoco.Unity.Framework.WorldBuilding.Components
{
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using UnityEngine;
    using UnityEngine.Events;
    using Zenject;

    public class WorldBuilder : MonoBehaviour, IWorldBuilder
    {
        [Inject]
        private IStructuredLog log;

        [Inject]
        private IWorldMap map;

        [Inject]
        private IBuildStepStore buildStepStore;

        public Component[] layers;

        public UnityEvent worldBuilt;

        public void Build()
        {
            this.AddBuildSteps();

            var buildSteps = this.buildStepStore.GetAll();

            for (int i = 0; i < buildSteps.Length; i++)
            {
                if (this.log.Status.IsTraceEnabled)
                    this.log.Trace("Building world layer step").Field("stepName", buildSteps[i].Name).Write();
                
                buildSteps[i].Build(this.map);
            }
        }

        private void AddBuildSteps()
        {
            if (this.log.Status.IsTraceEnabled)
                this.log.Trace("Adding world layer build steps").Field("numLayers", this.layers.Length).Write();
            
            for (int i = 0; i < this.layers.Length; i++)
            {
                var layerBuilder = (ILayerBuilder)this.layers[i];
                layerBuilder.AddBuildSteps(this.buildStepStore);
            }
        }
    }
}
