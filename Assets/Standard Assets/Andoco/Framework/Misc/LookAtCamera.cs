namespace Andoco.Unity.Framework.Misc
{
    using UnityEngine;

    public class LookAtCamera : MonoBehaviour
    {
        private Transform cachedTransform;

        public Camera targetCamera;

        void Awake()
        {
            this.cachedTransform = this.transform;

            if (this.targetCamera == null)
            {
                this.targetCamera = Camera.main;
            }
        }

        void LateUpdate()
        {
            this.cachedTransform.rotation = this.targetCamera.transform.rotation;
        }
    }
}