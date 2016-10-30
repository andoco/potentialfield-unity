namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

    public static class RectExtensions
    {
        public static Vector2[] ToPoly(this Rect rect)
        {
            return new [] {
                new Vector2(rect.xMax, rect.yMin),
                new Vector2(rect.xMax, rect.yMax),
                new Vector2(rect.xMin, rect.yMax),
                new Vector2(rect.xMin, rect.yMin)
            };
        }

        public static Rect[] SplitHorizontally(this Rect rect, int numSegments)
        {
            var rects = new Rect[numSegments];
            var itemWidth = rect.width / numSegments;

            for (int i = 0; i < numSegments; i++)
            {
                var r = new Rect();
                r.x = rect.x + i * itemWidth;
                r.y = rect.y;
                r.width = itemWidth;
                r.height = rect.height;

                rects[i] = r;
            }

            return rects;
        }

        public static Rect[] SplitVertically(this Rect rect, int numSegments, float spacing = 0f)
        {
            var rects = new Rect[numSegments];
            var itemHeight = rect.height / numSegments;

            for (int i = 0; i < numSegments; i++)
            {
                var r = new Rect();
                r.x = rect.x;
                r.y = rect.y + i * itemHeight;
                r.width = rect.width;
                r.height = itemHeight - spacing;

                rects[i] = r;
            }

            return rects;
        }
    }
}