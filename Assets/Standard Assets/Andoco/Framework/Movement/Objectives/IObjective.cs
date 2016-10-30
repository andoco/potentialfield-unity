namespace Andoco.Unity.Framework.Movement.Objectives
{
    using UnityEngine;

    public interface IObjective
    {
        /// <summary>
        /// Gets the data associated with the objective.
        /// </summary>
        /// <remarks>>
        /// Used to correlate a positional objective with some other entity.
        /// </remarks>
        object Data { get; }

        ObjectiveKind Kind { get; }

        Vector3 TargetPosition { get; }

        /// <summary>
        /// Validates the objective to check if it can still be used. Invalid objectives should be discarded.
        /// </summary>
        bool Validate();
    }
}
