namespace Andoco.Unity.Framework.Core.DebugConsole
{
    using System.Collections;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Commands;
    using Andoco.Core.Diagnostics.Commands.Lookup;
    using Andoco.Core.Diagnostics.Commands.Parsing;
    using Andoco.Unity.Framework.Data;
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.UI;
    using Zenject;

    public class DebugConsole : MonoBehaviour
    {
        private bool isOpen = true;
        private string historyDataKey = "debugConsoleHistory";

        [Inject]
        private IDebugCommandExecutor commandExecutor;

        [Inject]
        private AliasDebugCommand aliasCommand;

        [Inject]
        private IGameData data;

        private System.IO.StringWriter writer;
        private IList<string> commandHistory;
        private int commandHistoryIndex = 0;
        private bool browsingHistory;

        public ScrollRect scroll;
        public InputField input;
        public Text output;

        public DebugCommandAlias[] aliases;

        void Awake()
        {
            Assert.IsNotNull(this.input);
            Assert.IsNotNull(this.output);
            Assert.IsNotNull(this.scroll);

            this.writer = new System.IO.StringWriter();
        }

        [Inject]
        void OnPostInject()
        {
            this.commandHistory = this.data.GetOrAdd<List<string>>(this.historyDataKey);
            this.aliasCommand.Aliases = this.aliases;
        }

        void OnEnable()
        {
            if (this.commandHistory != null)
            {
                this.commandHistoryIndex = this.commandHistory.Count - 1;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && this.commandHistory.Count > 0)
            {
                this.EnterPreviousCommand();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && this.commandHistory.Count > 0)
            {
                this.EnterNextCommand();
            }
        }

        public void Toggle()
        {
            this.input.text = string.Empty;
            this.isOpen = !this.isOpen;
            this.browsingHistory = false;
            this.gameObject.SetActive(this.isOpen);

            if (this.isOpen)
                this.FocusInputField();
        }

        public void OnCommandEntered(string commandText)
        {
            // Ignore command if the console was closed.
            if (!this.isOpen || !this.gameObject.activeInHierarchy)
                return;

            Debug.LogFormat("Entered command: {0}", commandText);

            if (!string.IsNullOrEmpty(commandText))
            {
                commandText = this.ExpandAlias(commandText);

                this.commandExecutor.Execute(commandText, this.writer);
                StartCoroutine(UpdateOutputRoutine());

                if (this.commandHistory.Count == 0 || commandText != this.commandHistory[this.commandHistory.Count - 1])
                {
                    this.commandHistory.Add(commandText);
                }
            }

            this.FocusInputField();

            this.browsingHistory = false;
        }

        IEnumerator UpdateOutputRoutine()
        {
            this.input.text = string.Empty;
            this.output.text = this.writer.ToString();

            yield return null;

            this.scroll.verticalNormalizedPosition = 0f;
        }

        private void FocusInputField()
        {
            this.input.Select();
            this.input.ActivateInputField();
        }

        private bool CheckBrowsingHistory()
        {
            if (this.browsingHistory)
                return true;

            // Start new history browsing.
            this.commandHistoryIndex = this.commandHistory.Count - 1;
            this.browsingHistory = true;

            return false;
        }

        private void EnterPreviousCommand()
        {
            if (this.CheckBrowsingHistory())
                this.commandHistoryIndex = Mathf.Max(this.commandHistoryIndex - 1, 0);

            this.input.text = this.commandHistory[this.commandHistoryIndex];
        }

        private void EnterNextCommand()
        {
            if (this.CheckBrowsingHistory())
                this.commandHistoryIndex = Mathf.Min(this.commandHistoryIndex + 1, this.commandHistory.Count - 1);

            this.input.text = this.commandHistory[this.commandHistoryIndex];
        }

        private string ExpandAlias(string commandText)
        {
            for (int i = 0; i < this.aliases.Length; i++)
            {
                if (string.Equals(commandText, this.aliases[i].alias, System.StringComparison.OrdinalIgnoreCase))
                {
                    return this.aliases[i].commandText;
                }
            }

            return commandText;
        }
    }
}
