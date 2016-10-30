namespace Andoco.Unity.Framework.Core
{
	using System;
	using UnityEngine;

    public static class TransformEntityExtensions
    {
        /// <summary>
        /// Gets the component in self or ancestors.
        /// </summary>
        /// <returns>The component in self or ancestors.</returns>
        /// <param name="component">The component to start the search from.</param>
        /// <param name="depth">Depth to search for in ancestors. 0 will only search self, 1 will search self and parent and so on.</param>
        /// <typeparam name="T">The type of the component to search for.</typeparam>
        public static T GetComponentInParent<T>(this Transform currentTx, int depth = int.MaxValue) where T : Component
        {
            T foundComponent = null;

            while (foundComponent == null && currentTx != null && depth >= 0)
            {
                foundComponent = currentTx.gameObject.GetComponent<T>();

                currentTx = currentTx.parent;

                depth--;
            }

            return foundComponent;
        }

        /// <summary>
        /// Gets the <see cref="GameObject"/> that is the root of the entity.
        /// </summary>
        /// <returns>The entity GameObject, or <c>null</c> if no entity was found.</returns>
        /// <param name="component">Component to start searching from.</param>
        public static GameObject GetEntityGameObject(this Transform tx)
        {
            var entity = tx.GetComponentInParent<Entity>();

            if (entity == null)
                return null;

            return entity.gameObject;
        }
    }
    
}