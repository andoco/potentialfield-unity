using Zenject;
using Andoco.Unity.Framework.Installers;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Core.Logging;

public class FlowDemoInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Install<CoreInstaller>();
        Container.Install<LoggingInstaller>();
        Container.Install<ObjectPoolInstaller>();
    }
}