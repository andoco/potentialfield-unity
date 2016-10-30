namespace Andoco.Unity.Framework.Movement.Plan
{
    using UnityEngine;

    public class SimpleMovePlanStep : IMovePlanStep
    {
        public SimpleMovePlanStep()
        {
        }

        public SimpleMovePlanStep(Vector3 position)
        {
            this.Position = position;
        }

        public MovePlanStepType StepType { get { return MovePlanStepType.Path; } }

        public Vector3 Position { get; set; }
    }
}
