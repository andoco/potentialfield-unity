namespace Andoco.Unity.Framework.Core.Scene.Management
{
    using UnityEngine;

    public interface IGameObjectManager
    {
        /// <summary>
        /// Create a new GameObject based on the prefab.
        /// </summary>
        /// <param name="kind">The kind of gameobject to create.</param>
        /// <param name="label">The label to assign to the gameobject's Entity component.</param>
        GameObject Create(string kind, string label = null);

		/// <summary>
		/// Create a new GameObject based on the prefab.
		/// </summary>
		/// <param name="prefab">The prefab to create the gameobject from.</param>
		/// <param name="label">The label to assign to the gameobject's Entity component.</param>
		GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation, string label = null);

        void Destroy(GameObject gameObject);

        GameObject GetOrCreate(string kind, string label = null);

        GameObject GetOrCreate(GameObject prefab, string label = null);

        bool IsAlive(Object obj);

        bool TryGet(string kind, string label, out GameObject go);
    }
}
