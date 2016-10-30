namespace Andoco.Unity.Framework.WorldBuilding
{
    using System;

    public interface IBuildStep
    {
        string Name { get; }

        void Build(IWorldMap map);
    }
}
