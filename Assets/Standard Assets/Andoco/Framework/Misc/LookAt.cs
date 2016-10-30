namespace Andoco.Unity.Framework.Misc
{
    using UnityEngine;
    using System.Collections;

    public class LookAt : MonoBehaviour
    {
        public Transform target;

        void LateUpdate()
        {
            this.transform.LookAt(this.target);
        }
    }
}
