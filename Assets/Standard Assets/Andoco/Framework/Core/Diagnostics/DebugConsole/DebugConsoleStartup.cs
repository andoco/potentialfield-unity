namespace Andoco.Unity.Framework.Core.DebugConsole
{
    using Andoco.Core.Diagnostics.Commands;
    using Andoco.Core.Diagnostics.Commands.Lookup;
    using UnityEngine;
    using Zenject;

    public class DebugConsoleStartup : IInitializable
    {
        private readonly IResolver resolver;

        private readonly IDebugCommandSource commandSource;

        public DebugConsoleStartup(IResolver resolver, IDebugCommandSource commandSource)
        {
            this.commandSource = commandSource;
            this.resolver = resolver;
        }

        public void Initialize()
        {
            var allCommands = this.resolver.ResolveAll<IDebugCommand>();
            this.commandSource.AddAll(allCommands);
        }
    }
}
