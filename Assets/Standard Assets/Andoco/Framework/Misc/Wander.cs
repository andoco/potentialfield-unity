namespace Andoco.Unity.Framework.Misc
{
    using UnityEngine;
    using System.Collections;
    
    public class Wander : MonoBehaviour
    {
        public float speed = 1f;
        public float changeDirectionInterval = 5f;
        public float maximumTurnDegrees = 90f;
        public bool constrainToBounds;
        public Bounds constrainBounds;
    
        void Start()
        {
            StartCoroutine(ChangeDir());
        }
    
        void Update()
        {
            this.transform.Translate(Vector3.forward * this.speed * Time.deltaTime);

            if (this.constrainToBounds && !this.constrainBounds.Contains(this.transform.position))
            {
                this.transform.LookAt(Vector3.zero);
            }
        }
    
        private IEnumerator ChangeDir()
        {
            while (true) {
                this.transform.Rotate(Vector3.up, Random.Range(-this.maximumTurnDegrees, this.maximumTurnDegrees));
    
                yield return new WaitForSeconds(this.changeDirectionInterval);
            }
        }
    }
}
