namespace Andoco.Unity.Framework.WorldBuilding
{
    using System;

    public class ActionBuildStep : IBuildStep
    {
        private readonly Action<IWorldMap> buildAction;

        public ActionBuildStep(string name, Action<IWorldMap> buildAction)
        {
            this.Name = name;
            this.buildAction = buildAction;
        }

        public string Name { get; private set; }

        public void Build(IWorldMap map)
        {
            this.buildAction(map);
        }
    }
}
