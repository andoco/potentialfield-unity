using UnityEngine;
using UnityEngine.EventSystems;

namespace Andoco.Unity.Framework.Core
{
    public static class PointerEventDataExtensions
    {
        public static bool TryGetRaycastHit(this PointerEventData eventData, int layerMask, out RaycastHit hit)
        {
            var screenPos = eventData.position;
            var ray = Camera.main.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                return true;
            }

            hit = default(RaycastHit);
            return false;
        }

        public static bool TryGetRaycastHitPoint(this PointerEventData eventData, int layerMask, out Vector3 hitPoint)
        {
            RaycastHit hit;

            if (eventData.TryGetRaycastHit(layerMask, out hit))
            {
                hitPoint = hit.point;
                return true;
            }

            hitPoint = default(Vector3);
            return false;
        }
    }
}
