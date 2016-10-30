namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Andoco.Core;

    public interface IWaypoint
    {
        Vector3 Position { get; set; }

        int Flags { get; set; }
    }

}