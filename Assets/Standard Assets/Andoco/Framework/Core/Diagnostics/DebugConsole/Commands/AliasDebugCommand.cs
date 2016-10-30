namespace Andoco.Unity.Framework.Core.DebugConsole
{
    using System.Text.RegularExpressions;
    using Andoco.Core.Diagnostics.Commands;
    using UnityEngine;

    public class AliasDebugCommand : DebugCommand<AliasDebugCommand.Args>
    {
        public DebugCommandAlias[] Aliases { get; set; }

        #region implemented abstract members of DebugCommand

        protected override ExecutionResultKind OnCommand(Args args, System.IO.TextWriter output)
        {
            if (this.Aliases != null)
            {
                foreach (var alias in this.Aliases)
                {
                    output.WriteLine("{0} = {1}", alias.alias, alias.commandText);
                }
            }

            return ExecutionResultKind.Success;
        }

        #endregion
        
        public class Args
        {
        }
    }
}
