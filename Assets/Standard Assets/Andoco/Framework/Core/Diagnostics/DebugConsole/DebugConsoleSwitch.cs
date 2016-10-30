namespace Andoco.Unity.Framework.Core.DebugConsole
{
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.UI;

    public class DebugConsoleSwitch : MonoBehaviour
    {
        public DebugConsole console;
        public KeyCode toggleKey;

        void Start()
        {
            Assert.IsNotNull(this.console);

            // Hide on start.
            this.console.Toggle();
        }

        void Update()
        {
            if (Input.GetKeyDown(this.toggleKey))
            {
                this.console.Toggle();
            }
        }
    }
}
