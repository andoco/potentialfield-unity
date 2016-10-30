namespace Andoco.Unity.Framework.Movement.Plan
{
    using System.Collections.Generic;
    using UnityEngine;

    public enum MovePlanStepType
    {
        None,
        Simple,
        Path
    }

    public interface IMovePlan
    {
        bool IsEmpty { get; }

        IList<IMovePlanStep> Steps { get; }

        void Add(IMovePlanStep step);

        void Clear();
    }
}
