namespace Andoco.Unity.Framework.Core.DebugConsole
{
    using Andoco.Core.Diagnostics.Commands;
    using UnityEngine;
    using System.Text.RegularExpressions;

    public class ListMatchingDebugCommand : DebugCommand<ListMatchingDebugCommand.Args>
    {
        protected override ExecutionResultKind OnCommand(Args args, System.IO.TextWriter output)
        {
            if (string.IsNullOrEmpty(args.Pattern))
            {
                output.WriteLine("error: Requires Pattern argument");
                return ExecutionResultKind.Failure;
            }

            var all = Object.FindObjectsOfType<GameObject>();

            for (int i = 0; i < all.Length; i++)
            {
                var go = all[i];

                if (Regex.IsMatch(go.name, args.Pattern))
                {
                    this.WriteLine(go.name, output);
                }
            }

            return ExecutionResultKind.Success;
        }

        public class Args
        {
            [DebugCommandArg(Description = "The regular expression pattern to use")]
            public string Pattern { get; set; }
        }
    }
}

