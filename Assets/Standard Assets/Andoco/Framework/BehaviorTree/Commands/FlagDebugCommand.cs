using System;
using Andoco.BehaviorTree.Signals;
using Andoco.Core.Diagnostics.Commands;

namespace Andoco.Unity.Framework.BehaviorTree.Commands
{
    public class FlagDebugCommand : DebugCommand<FlagDebugCommand.Args>
    {
        private readonly FlagSignal signal;

        public FlagDebugCommand(FlagSignal signal)
        {
            this.signal = signal;
        }

        protected override ExecutionResultKind OnCommand(Args args, System.IO.TextWriter output)
        {
            this.signal.DispatchFlag(args.Name);

            return ExecutionResultKind.Success;
        }

        public class Args
        {
            [DebugCommandArg(Description = "The name of the global flag to dispatch")]
            public string Name { get; set; }
        }
    }
}
