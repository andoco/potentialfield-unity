using UnityEngine;
using System.Collections.Generic;

namespace Andoco.Unity.Framework.Core
{
    public static class ValidationListExtensions
    {
        /// <summary>
        /// Removes invalid objects from the list using <see cref="ObjectValidator"/>.
        /// </summary>
        /// <returns>The number of invalid objects removed.</returns>
        public static int RemoveInvalid<T>(this IList<T> objects) where T : Object
        {
            var numRemoved = 0;

            for (int i = objects.Count - 1; i >= 0; i--)
            {
                if (!ObjectValidator.Validate(objects[i]))
                {
                    objects.RemoveAt(i);
                    numRemoved++;
                }
            }

            return numRemoved;
        }
    }
}
