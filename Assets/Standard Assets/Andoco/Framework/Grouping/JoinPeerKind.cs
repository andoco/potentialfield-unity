namespace Andoco.Unity.Framework.Grouping
{
    public enum JoinPeerKind
    {
        /// <summary>
        /// Will not join any group.
        /// </summary>
        None,

        /// <summary>
        /// Leaves current group and joins the new one.
        /// </summary>
        Leave,

        /// <summary>
        /// Merges the current group with the new one.
        /// </summary>
        Merge
    }
}
