namespace Andoco.Unity.Framework.WorldBuilding.Components
{
    using UnityEngine;

    public class NullWorldLayer : MonoBehaviour, ILayerBuilder
    {
        public void AddBuildSteps(IBuildStepStore buildStepStore)
        {
        }
    }
}
