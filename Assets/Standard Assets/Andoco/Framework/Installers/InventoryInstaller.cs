namespace Andoco.Unity.Framework.Installers
{
    using Andoco.Unity.Framework.Inventory;
    using Zenject;

    public class InventoryInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IItemCatalog>().To<SimpleItemCatalog>().AsSingle();
        }
    }
}