namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using UnityEngine;
    using Andoco.Unity.Framework.Movement.Plan;
    
    public class WaypointPathMovePlanStep : IMovePlanStep
    {
        public WaypointPathMovePlanStep()
        {
        }

        public WaypointPathMovePlanStep(IWaypointPath path)
        {
            this.Path = path;
        }

        public MovePlanStepType StepType { get { return MovePlanStepType.Path; } }

        public IWaypointPath Path { get; set; }
    }
}