namespace Andoco.Unity.Framework.Core
{
	using System;
	using UnityEngine;

    public static class ComponentExtensions
    {
        /// <summary>
        /// Gets the component in self or parent.
        /// </summary>
        /// <returns>The component in self or parent.</returns>
        /// <param name="component">The component to start the search from.</param>
        /// <typeparam name="T">The type of the component to search for.</typeparam>
        [Obsolete("Use GetComponentInParent<T>() instead")]
        public static T GetComponentInParent<T>(this Component component) where T : Component
        {
            return component.GetComponentInParent<T>(1);
        }

        /// <summary>
        /// Gets the component in self or ancestors.
        /// </summary>
        /// <returns>The component in self or ancestors.</returns>
        /// <param name="component">The component to start the search from.</param>
        /// <param name="depth">Depth to search for in ancestors. 0 will only search self, 1 will search self and parent and so on.</param>
        /// <typeparam name="T">The type of the component to search for.</typeparam>
        public static T GetComponentInParent<T>(this Component component, int depth = int.MaxValue) where T : Component
        {
            Transform currentTx = component.transform;

            return currentTx.GetComponentInParent<T>();
        }

        /// <summary>
        /// Gets the <see cref="GameObject"/> that is the root of the entity.
        /// </summary>
        /// <returns>The entity GameObject, or <c>null</c> if no entity was found.</returns>
        /// <param name="component">Component to start searching from.</param>
        public static GameObject GetEntityGameObject(this Component component)
        {
            var entity = component.GetComponentInParent<Entity>();

            if (entity == null)
                return null;

            return entity.gameObject;
        }

        /// <summary>
        /// Gets the entity that the current component is a part of.
        /// </summary>
        /// <returns>The entity component, or <c>null</c> if no entity was found.</returns>
        /// <param name="component">Component to start searching from.</param>
        public static Entity GetEntityInParent(this Component component)
        {
            return component.GetComponentInParent<Entity>();
        }

        /// <summary>
        /// Gets the component in any <see cref="GameObject"/> that is part of the entity.
        /// </summary>
        /// <returns>The component in the entity, or <c>null</c> if no entity was found.</returns>
        /// <param name="component">Component to start searching from.</param>
        /// <typeparam name="T">The type of the component to search for.</typeparam>
        public static T GetComponentInEntity<T>(this Component component) where T : Component
        {
            var entity = component.GetEntityInParent();

            if (entity == null)
                return null;

            return entity.GetComponentInChildren<T>();
        }

        /// <summary>
        /// Gets the component in any <see cref="GameObject"/> that is part of the entity.
        /// </summary>
        /// <returns>The component in the entity, or <c>null</c> if no entity was found.</returns>
        /// <param name="component">Component.</param>
        /// <param name="type">The type of the component to search for.</param>
        public static Component GetComponentInEntity(this Component component, Type type)
        {
            var entity = component.GetEntityInParent();

            if (entity == null)
                return null;

            return entity.GetComponentInChildren(type);
        }
    }
}