namespace Andoco.Unity.Framework.Core
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Attribute to enable a property drawer for selecting a conrete type assignable to a specified base type.
    /// </summary>
    public class TypeReferenceAttribute : PropertyAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Andoco.Unity.Framework.Core.TypeReferenceAttribute"/> class.
        /// </summary>
        /// <param name="referencedBaseType">The base type used to search for concrete types.</param>
        public TypeReferenceAttribute(Type referencedBaseType, bool nullOption = true)
        {
            this.ReferencedBaseType = referencedBaseType;
            this.NullOption = nullOption;
        }

        /// <summary>
        /// Gets the base type for which we want to find concrete implementations.
        /// </summary>
        public Type ReferencedBaseType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the type reference allows <c>null</c> value to be chosen.
        /// </summary>
        public bool NullOption { get; private set; }
    }
}