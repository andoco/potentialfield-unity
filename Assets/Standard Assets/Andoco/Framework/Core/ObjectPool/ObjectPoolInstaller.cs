namespace Andoco.Unity.Framework.Core
{
    using Andoco.Unity.Framework.Core.ObjectPooling.Creator;
    using Zenject;

    public class ObjectPoolInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IGameObjectCreator>().To<ZenjectGameObjectCreator>().AsSingle();
            Container.Bind<IObjectPool>().To<ObjectPool>().AsSingle();
        }
    }
}
