namespace Andoco.Unity.Framework.Core
{
    using System.Linq;
    using UnityEngine;
	using UnityEngine.SceneManagement;

    public class SceneSingleton : MonoBehaviour
    {
        void Start()
        {
            var existing = GameObject.FindObjectsOfType<SceneSingleton>();

            if (existing.Any(x => x != this && x.gameObject.name == this.gameObject.name))
            {
                GameObject.Destroy(this.gameObject);
                return;
            }

            // DontDestroyOnLoad only works on objects at the scene root.
            this.transform.parent = null;

            DontDestroyOnLoad(this.gameObject);
        }
    }
}