namespace Andoco.Unity.Framework.Misc
{
    using UnityEngine;
    using System.Collections;

    [AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
    public class MouseOrbitImproved : MonoBehaviour {

        private float lastDist;

        public Transform target;
        public float distance = 5.0f;
        public float xSpeed = 120.0f;
        public float ySpeed = 120.0f;

        public float yMinLimit = -20f;
        public float yMaxLimit = 80f;

        public float distanceMin = .5f;
        public float distanceMax = 15f;

        public bool requireMouseButton;

        private bool firstUpdate;

        float x = 0.0f;
        float y = 0.0f;

        // Use this for initialization
        void Start () 
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            var rigidbody = GetComponent<Rigidbody>();

            // Make the rigid body not change rotation
            if (rigidbody != null)
            {
                rigidbody.freezeRotation = true;
            }

            firstUpdate = true;
        }

        void LateUpdate () 
        {
            if (target) 
            {
                if (SystemInfo.deviceType == DeviceType.Handheld)
                {
                    this.UpdateTouch();
                }
                else
                {
                    this.UpdateMouse();
                }

                firstUpdate = false;
            }
        }

        private void UpdateCommon(Quaternion rotation)
        {
            RaycastHit hit;
            if (Physics.Linecast (target.position, transform.position, out hit)) 
            {
                distance -=  hit.distance;
            }
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
        }

        private void UpdateTouch()
        {
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                var delta = Input.GetTouch(0).deltaPosition;
                var dist = delta.magnitude;
                x += delta.x * xSpeed * dist * 0.02f;
                y += -delta.y * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }
            else if (Input.touchCount >= 2)
            {
                var touch0 = Input.GetTouch(0).position;
                var touch1 = Input.GetTouch(1).position;
                var curDist = Vector2.Distance(touch0, touch1);

                if (lastDist > 0f)
                {
                    var deltaDist = curDist - lastDist;
                    distance = Mathf.Clamp(distance - deltaDist, distanceMin, distanceMax);
                }

                lastDist = curDist;
            }

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                lastDist = 0f;
            }

            this.UpdateCommon(rotation);
        }

        private void UpdateMouse()
        {
            var checkButton = firstUpdate || !this.requireMouseButton || Input.GetMouseButton(0);

            if (checkButton)
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel")*5, distanceMin, distanceMax);

            this.UpdateCommon(rotation);
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}

