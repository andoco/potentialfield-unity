namespace Andoco.Unity.Framework.Misc
{
    using UnityEngine;
    using System;
    using System.Collections;

    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsField : MonoBehaviour
    {
        private Transform cachedTransform;
        private Rigidbody cachedBody;

        public FieldKind field;
        public RadialGravitySettings radialGravity;

        void Awake()
        {
            this.cachedTransform = this.transform;
            this.cachedBody = this.GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            switch (this.field)
            {
                case FieldKind.None:
                    break;
                case FieldKind.RadialGravity:
                    this.UpdateRadialGravity();
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown field kind {0}", this.field));
            }
        }

        private void UpdateRadialGravity()
        {
            var settings = this.radialGravity;
            var pos = this.cachedTransform.position;
            var colliders = Physics.OverlapSphere(pos, settings.maxRadius, settings.layers.value);

            for (int i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];
                var body = collider.attachedRigidbody;

                if (body == null)
                {
                    body = collider.GetComponentInParent<Rigidbody>();
                }

                if (body != null)
                {
                    var offset = pos - body.position;

                    if (settings.simple)
                    {
                        var f = offset.normalized * settings.simpleGravityForce;
                        body.AddForce(f, ForceMode.Acceleration);
                    }
                    else
                    {
                        var f = offset.normalized * ((this.cachedBody.mass * body.mass) / (offset.sqrMagnitude));
                        body.AddForce(f, ForceMode.Force);
                    }
                }
            }
        }

        public enum FieldKind
        {
            None,
            RadialGravity
        }

        [Serializable]
        public struct RadialGravitySettings
        {
            public LayerMask layers;
            public float maxRadius;
            public bool simple;
            public float simpleGravityForce;
        }
    }
}
