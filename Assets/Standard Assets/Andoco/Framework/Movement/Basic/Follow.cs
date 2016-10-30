namespace Andoco.Unity.Framework.Movement.Basic
{
	using UnityEngine;
	using System.Collections;

	public class Follow : MonoBehaviour {
	    private Vector3 currentTargetPos;
	    private Transform thisTransform;

	    // The object to follow.
	    public Transform target;
	    
	    // If target is stationary, a value 1.0 will catch up in 1 second. 0.5 in 2.0 seconds. 0.0 will never catch up.
	    public float smoothTime = 0.3f;
	    
	    // Offset from the target to follow.
	    public Vector3 offset = Vector3.zero;
	    
	    // Is the offset relative to the target in its local space.
	    public bool localOffset = true;

	    public float maxSpeed = 0f;

	    void Awake()
	    {
	        thisTransform = transform;
	    }

	    void Recycled()
	    {
	        target = null;
	    }
	    
	    void LateUpdate()
	    {
	        if (target != null)
	        {
	            UpdateTargetPos();

	            if (this.maxSpeed > 0f)
	            {
	                var targetOffset = currentTargetPos - thisTransform.position;
	                var delta = Vector3.Lerp(Vector3.zero, targetOffset, Time.deltaTime * smoothTime);
	                if (delta.magnitude > this.maxSpeed * Time.deltaTime)
	                    delta = delta.normalized * this.maxSpeed * Time.deltaTime;
	                thisTransform.position += delta;
	            }
	            else
	            {
	                // TOOD: detect terrain elevation
	                thisTransform.position = Vector3.Lerp(thisTransform.position, currentTargetPos, Time.deltaTime * smoothTime);
	            }
	        }
	    }

	    void OnDrawGizmos()
	    {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(thisTransform.position, currentTargetPos);
                Gizmos.DrawWireSphere(currentTargetPos, 1f);
            }
	    }

	    private void UpdateTargetPos()
	    {
	        if (localOffset)
	        {
	            // Convert offset from local space to world space
	            this.currentTargetPos = target.TransformPoint(offset);
	        }
	        else
	        {
	            // Add offset to target position
	            this.currentTargetPos = target.position + offset;
	        }
	    }
	}
}