using System.Collections;
using System.IO;
using Andoco.Core.Diagnostics.Commands;
using Andoco.Unity.Framework.Core;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class TogglePotentialFieldDebugCommand : DebugCommand<TogglePotentialFieldDebugCommand.Args>
    {
        protected override ExecutionResultKind OnCommand(Args args, TextWriter output)
        {
            var sys = GameObject.FindObjectOfType<PotentialFieldSystem>();

            if (sys == null)
                return ExecutionResultKind.Failure;
            
            sys.ToggleDebug();

            return ExecutionResultKind.Success;
        }

        public class Args
        {
        }
    }
}
