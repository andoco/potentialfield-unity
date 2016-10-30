using System;
using Andoco.Core;
using Andoco.Core.Reflection;
using Andoco.Core.Signals;
using Andoco.Unity.Framework.Core;
using Zenject;

namespace Andoco.Unity.Framework.Installers
{
    public class CoreInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Unbind<IRandomNumber>();
            Container.Bind<IRandomNumber>().To<UnityRandomNumber>().AsSingle();

            Container.Unbind<ITime>();
            Container.Bind<ITime>().To<UnityTime>().AsSingle();

            Container.Bind<ITickable>().To<ZenjectTickedAdapter>().AsSingle();

            Container.Bind<ITypeFactory>().To<ZenjectTypeFactory>().AsSingle();

            Container.Bind<ITypeIndex>().To<ConcurrentTypeIndex>().AsSingle();

            Container.Bind<IComponentIndex>().To<DefaultComponentIndex>().AsSingle();

            Container.Bind<IInitializable>().To<CoreStartup>().AsSingle();

            // Signals from all assemblies.
            Container.Bind<ISignal>().To(x => x.AllTypes().DerivingFrom<ISignal>()).AsSingle();
            Container.Bind<SimpleSignal>().To(x => x.AllTypes().DerivingFrom<SimpleSignal>()).AsSingle();
            Container.Bind(x => x.AllNonAbstractClasses().DerivingFrom<ISignal>()).ToSelf().AsSingle();
            Container.Bind<ISignalBoard>().To<ZenjectSignalBoard>().AsSingle();
        }
    }

    public class CoreStartup : IInitializable
    {
        ITypeIndex typeIndex;

        public CoreStartup(ITypeIndex typeIndex)
        {
            this.typeIndex = typeIndex;
        }

        public void Initialize()
        {
            TypeIndex.Current = this.typeIndex;
        }
    }
}
