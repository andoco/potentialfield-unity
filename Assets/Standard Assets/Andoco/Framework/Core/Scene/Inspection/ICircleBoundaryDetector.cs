namespace Andoco.Unity.Framework.Core.Scene.Inspection
{
    using UnityEngine;

    public interface ICircleBoundaryDetector
    {
        /// <summary>
        /// Detects the radius of a circle that fits around the objects in the detection area.
        /// </summary>
        /// <param name="origin">Origin of the detection area.</param>
        /// <param name="normal">The plane axis on which the circle exists.</param>
        /// <param name="startRadius">Start radius of the detection area.</param>
        /// <param name="radiusStep">Amount to reduce the radius by when no objects are found in the detection area.</param>
        /// <param name="maxAttempts">The maximum number of times to reduce the radius by <paramref name="radiusStep"/>.</param>
        float Detect(Vector3 origin, Vector3 normal, float startRadius, float radiusStep, int maxAttempts = -1);
    }
}
