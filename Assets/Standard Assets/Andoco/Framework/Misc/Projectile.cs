namespace Andoco.Unity.Framework.Misc
{
    using UnityEngine;
    using System.Collections;
    using Andoco.Unity.Framework.Core;

    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour, IAimable
    {
        private Transform cachedTransform;
        private Rigidbody cachedBody;

        public ProjectileKind mode;
        public Transform target;
        public LinearVelocitySettings linear;
        public MissileSettings missile;
        public CannonSettings cannon;

        [Tooltip("The direction in local coordinates in which the projectile will move")]
        public bool launchOnStart;

        public bool IsLaunched { get; private set; }

        public bool IsThrusting { get; private set; }

        public Rigidbody RigidBody { get { return this.cachedBody; } }

        #region Lifecycle

        void Awake()
        {
            this.cachedTransform = this.transform;
            this.cachedBody = this.GetComponent<Rigidbody>();
        }

        void Start()
        {
            if (this.launchOnStart)
            {
                this.Launch();
            }
        }

        void Spawned()
        {
            if (this.launchOnStart)
            {
                this.Launch();
            }
        }

        void FixedUpdate()
        {
            if (this.IsLaunched)
            {
                switch (this.mode)
                {
                    case ProjectileKind.None:
                        break;
                    case ProjectileKind.LinearVelocity:
                        this.UpdateLinearVelocity();
                        break;
                    case ProjectileKind.Missile:
                        this.UpdateMissile();
                        break;
                    case ProjectileKind.Cannon:
                        break;
                    default:
                        throw new System.InvalidOperationException(string.Format("Unknown projectile mode {0}", this.mode));
                }
            }
        }

        void Recycled()
        {
            this.cachedBody.Sleep();

            this.IsLaunched = false;
            this.IsThrusting = false;
        }

        #endregion

        #region Public methods

        public void Launch()
        {
            if (this.IsLaunched)
                return;
    
            this.IsLaunched = true;
            this.IsThrusting = true;

            switch (this.mode)
            {
                case ProjectileKind.Cannon:
                    this.LaunchCannon();
                    break;
            }
        }

        public void Stall()
        {
            this.IsThrusting = false;
        }

        public void Aim(Transform target)
        {
            this.target = target;
        }

        #endregion

        #region Missile

        private void UpdateLinearVelocity()
        {
            if (this.target != null)
                this.cachedTransform.LookAt(this.target);
            
            this.cachedTransform.Translate(Vector3.forward * this.linear.speed * Time.deltaTime, Space.Self);
        }

        private void UpdateMissile()
        {
            if (this.target != null)
                this.cachedTransform.LookAt(this.target);
            
            var force = this.IsThrusting
                ? this.cachedTransform.TransformDirection(this.missile.direction) * this.missile.thrust
                : Vector3.zero;

            this.cachedBody.AddForce(force, ForceMode.Acceleration);
        }

        #endregion

        #region Cannon

        private void LaunchCannon()
        {
            if (this.target != null)
                this.cachedTransform.LookAt(this.target);
            
            this.cachedBody.AddForce(this.cannon.direction * this.cannon.force, ForceMode.Impulse);
        }

        #endregion

        #region Types

        public enum ProjectileKind
        {
            None,
            LinearVelocity,
            Missile,
            Cannon
        }

        [System.Serializable]
        public class LinearVelocitySettings
        {
            [Tooltip("The direction in local coordinates in which the projectile will move")]
            public Vector3 direction = Vector3.forward;

            [Tooltip("The constant velocity of the projectile")]
            public float speed;
        }

        [System.Serializable]
        public class MissileSettings
        {
            [Tooltip("The direction in local coordinates in which the projectile will move")]
            public Vector3 direction = Vector3.forward;

            [Tooltip("The thrust force applied in the direction of travel")]
            public float thrust;
        }

        [System.Serializable]
        public class CannonSettings
        {
            public Vector3 direction;
            public float force;
        }

        #endregion
    }
}
