namespace Andoco.Unity.Framework.Input
{
    using UnityEngine;
    using Zenject;
    using Andoco.Core.Signals;

    public class PanSignal : Signal<PanSignal.PanData>
    {
        [System.Flags]
        public enum PanState
        {
            None = 1 << 0,
            Began = 1 << 1,
            Continue = 1 << 2,
            Ended = 1 << 4
        }

        public struct PanData
        {
            public PanData(PanState state, Vector2 delta, Vector2 value, Vector2 absolute, Transform target)
                : this()
            {
                this.state = state;
                this.delta = delta;
                this.value = value;
                this.absolute = absolute;
                this.target = target;
            }

            /// <summary>
            /// The current state of the pan input.
            /// </summary>
            public PanState state;
    
            /// <summary>
            /// The normalized pan delta vector since the last pan callback.
            /// </summary>
            public Vector2 delta;
    
            /// <summary>
            /// The normalized pan vector since the pan began.
            /// </summary>
            public Vector2 value;
    
            /// <summary>
            /// The current absolute pan position in screen space.
            /// </summary>
            public Vector2 absolute;
    
            /// <summary>
            /// The starting target of the current pan.
            /// </summary>
            public Transform target;
        }
    }
}
