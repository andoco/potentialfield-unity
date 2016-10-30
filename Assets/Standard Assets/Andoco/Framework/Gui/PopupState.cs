namespace Andoco.Unity.Framework.Gui
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class PopupState
    {
        public PopupState(IEnumerable<string> labels)
        {
            this.Options = labels.Select(x => new GUIContent(x)).ToArray();
        }

        public GUIContent[] Options { get; private set; }

        public int SelectedIndex { get; private set; }

        public GUIContent SelectedOption { get; private set; }

        public string SelectedName { get { return this.SelectedOption == null ? null : this.SelectedOption.text; } }

        public void SetSelected(string label)
        {
            this.SetSelected(Array.FindIndex(this.Options, x => x.text == label));
        }

        public void SetSelected(int index)
        {
            var valid = index >= 0 && this.Options.Length > index;

            this.SelectedIndex = valid ? index : -1;
            this.SelectedOption = valid ? this.Options[index] : null;
        }
    }
}
