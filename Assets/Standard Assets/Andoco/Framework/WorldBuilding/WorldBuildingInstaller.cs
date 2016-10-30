namespace Andoco.Unity.Framework.WorldBuilding
{
    using Zenject;

    public class WorldBuildingInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IWorldMap>().To<WorldMap>().AsSingle();
            Container.Bind<IBuildStepStore>().To<DefaultBuildStepStore>().AsSingle();
        }
    }
}
