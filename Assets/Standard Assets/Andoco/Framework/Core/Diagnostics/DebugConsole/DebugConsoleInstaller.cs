namespace Andoco.Unity.Framework.Core.DebugConsole
{
    using Andoco.Core.Diagnostics.Commands;
    using Andoco.Core.Diagnostics.Commands.Lookup;
    using Andoco.Core.Diagnostics.Commands.Parsing;
    using Andoco.Core.Diagnostics.Commands.Standard;
    using UnityEngine;
    using Zenject;

    public class DebugConsoleInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindAllInterfaces<DefaultDebugCommandSource>().To<DefaultDebugCommandSource>().AsSingle();
            Container.BindAllInterfaces<DefaultDebugCommandFinder>().To<DefaultDebugCommandFinder>().AsSingle();
            Container.BindAllInterfaces<SimpleDebugCommandArgumentParser>().To<SimpleDebugCommandArgumentParser>().AsSingle();
            Container.BindAllInterfaces<DebugCommandExecutor>().To<DebugCommandExecutor>().AsSingle();

            Container.Bind<IDebugCommand>().To(x => x.AllNonAbstractClasses().DerivingFrom<IDebugCommand>()).AsSingle();
            Container.Bind(x => x.AllNonAbstractClasses().DerivingFrom<IDebugCommand>()).ToSelf().AsSingle();

            Container.Bind<IInitializable>().To<DebugConsoleStartup>().AsSingle();
        }
    }
}
