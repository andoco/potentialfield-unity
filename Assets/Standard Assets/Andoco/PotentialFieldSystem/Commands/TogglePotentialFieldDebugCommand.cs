using System.IO;
using Andoco.Core.Diagnostics.Commands;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class TogglePotentialFieldDebugCommand : DebugCommand<TogglePotentialFieldDebugCommand.Args>
    {
        protected override ExecutionResultKind OnCommand(Args args, TextWriter output)
        {
            var debugModule = GameObject.FindObjectOfType<PotentialFieldDebugModule>();

            if (debugModule == null)
                return ExecutionResultKind.Failure;

            debugModule.ToggleDebug();

            return ExecutionResultKind.Success;
        }

        public class Args
        {
        }
    }
}
