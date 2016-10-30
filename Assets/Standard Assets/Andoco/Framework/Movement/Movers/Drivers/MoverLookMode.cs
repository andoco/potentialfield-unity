namespace Andoco.Unity.Framework.Movement.Movers.Drivers
{
    public enum MoverLookMode
    {
        None,

        /// <summary>
        /// The up direction in world space.
        /// </summary>
        WorldUp,

        /// <summary>
        /// The up direction of the transform's local space.
        /// </summary>
        TransformUp
    }
}