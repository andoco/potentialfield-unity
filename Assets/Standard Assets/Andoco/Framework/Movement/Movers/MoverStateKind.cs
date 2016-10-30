namespace Andoco.Unity.Framework.Movement.Movers
{
    using UnityEngine;
    using System.Collections;
    using Andoco.Unity.Framework.Core;

    public enum MoverStateKind
    {
        /// <summary>
        /// The mover has not performed any movement yet.
        /// </summary>
        None,

        /// <summary>
        /// The mover has begun moving.
        /// </summary>
        Moving,

        /// <summary>
        /// The mover has been temporarily paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The mover has arrived at its final destination.
        /// </summary>
        Arrived,

        /// <summary>
        /// The mover was cancelled while it was moving.
        /// </summary>
        Cancelled
    }
}