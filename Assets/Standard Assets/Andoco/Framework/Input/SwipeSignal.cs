namespace Andoco.Unity.Framework.Input
{
    using UnityEngine;
    using Andoco.Core.Signals;

    public class SwipeSignal : Signal<SwipeSignal.Data>
    {
        public struct Data
        {
            public Transform Target { get; set; }

            public SwipeDirection Direction { get; set; }
        }
    }
}