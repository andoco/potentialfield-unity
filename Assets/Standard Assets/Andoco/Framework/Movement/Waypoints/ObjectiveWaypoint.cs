namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using UnityEngine;
    using Andoco.Unity.Framework.Movement.Objectives;

    public class ObjectiveWaypoint : IWaypoint
    {
        public ObjectiveWaypoint(IObjective objective)
        {
            this.Objective = objective;
        }

        public IObjective Objective { get; private set; }

        public Vector3 Position
        {
            get
            {
                return this.Objective.TargetPosition;
            }
            set
            {
                throw new NotSupportedException("ObjectiveWaypoint does not support changing the waypoint position directly. Change the position of the objective instead.");
            }
        }

        public int Flags { get; set; }

        public override string ToString()
        {
            return string.Format("[ObjectiveWaypoint: Objective={0}, Flags={1}]", Objective, Flags);
        }
    }
}