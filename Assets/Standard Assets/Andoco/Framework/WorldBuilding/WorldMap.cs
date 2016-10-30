namespace Andoco.Unity.Framework.WorldBuilding
{
    using Andoco.Core.Data;

    public class WorldMap : IWorldMap
    {
        public WorldMap()
        {
            this.Data = new UniqueTypeStore();
        }

        public UniqueTypeStore Data { get; private set; }
    }
}