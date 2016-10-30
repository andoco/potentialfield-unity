namespace Andoco.Unity.Framework.Misc
{
    using UnityEngine;
    using System.Collections;

    public class RotateAround : MonoBehaviour
    {
        public Transform origin;

        [Tooltip("The orbital speed in degrees/second")]
        public float speed;

        void LateUpdate()
        {
            var angle = this.speed * Time.deltaTime;

            if (this.origin == null || this.origin == this.transform)
            {
                this.transform.Rotate(Vector3.up, angle);
            }
            else
            {
                this.transform.RotateAround(this.origin.transform.position, Vector3.up, angle);
            }
        }
    }
}
