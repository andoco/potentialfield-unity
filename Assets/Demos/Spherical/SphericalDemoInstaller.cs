using Zenject;
using Andoco.Unity.Framework.Installers;
using Andoco.Unity.Framework.Core.Logging;
using Andoco.Unity.Framework.Core;

namespace Andoco.Unity.Framework.Demos
{
    public class SphericalDemoInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Install<CoreInstaller>();
            Container.Install<LoggingInstaller>();
            Container.Install<ObjectPoolInstaller>();
        }
    }
}