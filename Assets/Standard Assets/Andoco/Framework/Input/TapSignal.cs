namespace Andoco.Unity.Framework.Input
{
    using UnityEngine;
    using Andoco.Core.Signals;

    public class TapSignal : Signal<TapSignal.TapData>
    {
        public struct TapData
        {
            public TapData(Transform target, Vector2 position, int taps = 1)
            {
                this.target = target;
                this.position = position;
                this.taps = taps;
            }

            public Transform target;
    
            public Vector2 position;

            public int taps;
        }
    }
}