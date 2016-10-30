namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

    /// <summary>
    /// Attribute to mark a field as read-only, and prevent editing in the inspector.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute
    {
        /// <summary>
        /// Gets or sets a value indicating whether the field is read-only at runtime, but editable at design time.
        /// runtime only.
        /// </summary>
        /// <value><c>true</c> if readonly at runtime only; otherwise, <c>false</c>.</value>
        public bool RuntimeOnly { get; set; }
    }
}
