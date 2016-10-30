namespace Andoco.Unity.Framework.Core.Scene
{
    using UnityEngine;

    public interface ISceneOrganizer
    {
        /// <summary>
        /// Organize the specified instance in the scene based on its prefab.
        /// </summary>
        /// <param name="instance">The instance to organize.</param>
        /// <param name="prefab">The prefab used to create the instance.</param>
        void Organize(Transform instance, GameObject prefab);

        /// <summary>
        /// Cleanup the scene by removing any empty prefab parent <see cref="Transform"/> instances.
        /// </summary>
        void Cleanup();
    }
}