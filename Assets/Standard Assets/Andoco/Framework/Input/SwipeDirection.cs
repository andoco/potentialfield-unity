namespace Andoco.Unity.Framework.Input
{
    using UnityEngine;
    using Andoco.Core.Signals;

    [System.Flags]
    public enum SwipeDirection
    {
        Left = (1 << 0),
        Right = (1 << 1),
        Up = (1 << 2),
        Down = (1 << 4),
        Horizontal = (Left | Right),
        Vertical = (Up | Down),
        All = (Horizontal | Vertical)
    }
}
