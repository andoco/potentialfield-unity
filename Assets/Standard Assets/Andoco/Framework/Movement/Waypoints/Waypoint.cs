namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Andoco.Core;

    public class Waypoint : IWaypoint
    {
        public Waypoint()
        {
        }

        public Waypoint(Vector3 pos)
        {
            this.Position = pos;
        }

        public Vector3 Position { get; set; }

        public int Flags { get; set; }

        public override string ToString()
        {
            return string.Format("[Waypoint: Position={0}, Flags={1}]", Position, Flags);
        }
    }
}