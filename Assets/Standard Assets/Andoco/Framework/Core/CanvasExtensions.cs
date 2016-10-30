namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

    public static class CanvasExtensions
    {
        public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPos)
        {
            var canvasRect = canvas.GetComponent<RectTransform>(); // This is slow, so avoid using in an update.
            var viewportPos = Camera.main.WorldToViewportPoint(worldPos);
            var scaledCanvasSize = new Vector2(canvasRect.sizeDelta.x * canvasRect.localScale.x, canvasRect.sizeDelta.y * canvasRect.localScale.y);
            var screenPos = new Vector2(viewportPos.x * scaledCanvasSize.x, viewportPos.y * scaledCanvasSize.y);

            return screenPos;
        }
    }
}