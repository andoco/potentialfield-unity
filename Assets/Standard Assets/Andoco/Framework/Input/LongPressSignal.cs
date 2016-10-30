namespace Andoco.Unity.Framework.Input
{
    using UnityEngine;
    using Andoco.Core.Signals;

    public class LongPressSignal : Signal<LongPressSignal.LongPressData>
    {
        [System.Flags]
        public enum States
        {
            None = 1 << 0,
            Began = 1 << 1,
            Ended = 1 << 2
        }

        public struct LongPressData
        {
            public LongPressData(Transform target, Vector2 position, States state)
                : this()
            {
                this.target = target;
                this.position = position;
                this.state = state;
            }

            public Transform target;

            public Vector2 position;

            public States state;
        }
    }
}