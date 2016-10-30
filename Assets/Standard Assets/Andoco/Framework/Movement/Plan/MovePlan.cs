namespace Andoco.Unity.Framework.Movement.Plan
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MovePlan : IMovePlan
    {
        public MovePlan()
        {
            this.Steps = new List<IMovePlanStep>();
        }

        public bool IsEmpty { get { return this.Steps.Count == 0; } }

        public IList<IMovePlanStep> Steps { get; private set; }

        public void Add(IMovePlanStep step)
        {
            this.Steps.Add(step);
        }

        public void Clear()
        {
            this.Steps.Clear();
        }
    }
}
