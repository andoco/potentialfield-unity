namespace Andoco.Unity.Framework.Input
{
    using UnityEngine;
    using Andoco.Core.Signals;

    public class InputSourceSignal : Signal<InputSourceSignal.Data>
    {
        public struct Data
        {
            public bool Enabled { get; set; }

            public InputType InputType { get; set; }

            public Transform Target { get; set; }

            public SwipeDirection Direction { get; set; }
        }
    }
}
