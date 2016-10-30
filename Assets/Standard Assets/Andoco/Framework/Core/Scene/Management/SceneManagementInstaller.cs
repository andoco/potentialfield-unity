namespace Andoco.Unity.Framework.Core.Scene.Management
{
    using System;
    using UnityEngine;
    using Andoco.Unity.Framework.Installers;
    using Zenject;

    public class SceneManagementInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (!Container.HasInstalled<ObjectPoolInstaller>())
                Container.Install<ObjectPoolInstaller>();

            Container.Bind<IGameObjectManager>().To<GameObjectManager>().AsSingle();
            Container.Bind<IGameObjectStore>().To<DefaultGameObjectStore>().AsSingle();
            Container.Bind<IPrefabSelector>().To<PrefabRegistryPrefabSelector>().AsSingle();

            if (!Container.HasBinding<IPrefabRegistry>())
                Container.Bind<IPrefabRegistry>().To<DefaultPrefabRegistry>().AsSingle();
        }
    }
}
