namespace Andoco.Unity.Framework.Core
{
	using System;
	using UnityEngine;

    public static class GameObjectEntityExtensions
    {
        #region Entity

        /// <summary>
        /// Gets the <see cref="GameObject"/> that is the root of the entity.
        /// </summary>
        /// <returns>The entity GameObject, or <c>null</c> if no entity was found.</returns>
        /// <param name="component">Component to start searching from.</param>
        public static GameObject GetEntityGameObject(this GameObject go)
        {
            return go.transform.GetEntityGameObject();
        }

        /// <summary>
        /// Gets the entity that the current component is a part of.
        /// </summary>
        /// <returns>The entity component, or <c>null</c> if no entity was found.</returns>
        /// <param name="component">Component to start searching from.</param>
        public static Entity GetEntityInParent(this GameObject go)
        {
            return go.transform.GetComponentInParent<Entity>();
        }

        /// <summary>
        /// Gets the component in any <see cref="GameObject"/> that is part of the entity.
        /// </summary>
        /// <returns>The component in the entity, or <c>null</c> if no entity was found.</returns>
        /// <param name="component">Component to start searching from.</param>
        /// <typeparam name="T">The type of the component to search for.</typeparam>
        public static T GetComponentInEntity<T>(this GameObject go) where T : Component
        {
            var entity = go.GetEntityInParent();

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
        public static Component GetComponentInEntity(this GameObject go, Type type)
        {
            var entity = go.GetEntityInParent();

            if (entity == null)
                return null;

            return entity.GetComponentInChildren(type);
        }

        #endregion
    }
}