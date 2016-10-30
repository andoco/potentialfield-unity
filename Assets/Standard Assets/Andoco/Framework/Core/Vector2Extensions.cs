namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;
    using UnityEngine.Assertions;

    public static class Vector2Extensions
    {
        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v.x, v.y, 0f);
        }

        /// <summary>
        /// Converts a polar coordinate (lat/long) into a 3D cartesian coordinate.
        /// </summary>
        /// <remarks>>
        /// Taken from http://answers.unity3d.com/questions/189724/polar-spherical-coordinates-to-xyz-and-vice-versa.html.
        /// 
        /// The direction in which polar coordinates get mapped is controlled by the default rotation direction when using
        /// Unity's math methods. Positive will be clockwise, and negative anti-clockwise.
        /// 
        /// A positive polar x (latitude) value will mapped in the southern direction, and negative in the northern direction.
        /// 
        /// A positive polar y (longitutde) value will be mapped in the western direction, and negative to the east.
        /// </remarks>
        /// <returns>The cartesian coordinate.</returns>
        /// <param name="polar">The polar coordinate where x = latitude and y = longitude.</param>
        public static Vector3 PolarToCartesian(this Vector2 polar, float radius = 1f)
        {
            //an origin vector, representing lat,lon of 0,0. 

            var origin= new Vector3(radius,0,0);

            //build a quaternion using euler angles for lat,lon
            var rotation = Quaternion.Euler(0,polar.y,-polar.x);
            //transform our reference vector by the rotation. Easy-peasy!
            var point =rotation * origin;

            return point;
        }

        /// <summary>
        /// Converts a polar coordinate to a 2D UV coordinate in rectangular space.
        /// </summary>
        /// <remarks>
        /// A polar coordinate of (0,0) will give a UV coordinate in the centre of the UV coordinate space.
        /// </remarks>
        /// <returns>The UV coordinates.</returns>
        /// <param name="polar">The polar coordinate to get the UV coordinates for.</param>
        /// <param name="size">The size of the UV coordinate space (i.e. the texture size).</param>
        public static Vector2 PolarToUV(this Vector2 polar, Vector2 size)
        {
            var halfWidth = size.x / 2f;
            var halfHeight = size.y / 2f;

            var u = halfWidth + (halfWidth / 180f * polar.y);
            var v = halfHeight + (halfHeight / 90f * polar.x);

            if (u == size.x)
                u = 0;
            if (v == size.y)
                v = 0;

            return new Vector2(u, v);
        }

        /// <summary>
        /// Rotates a polar coordinate by the given latitude and longitude.
        /// </summary>
        /// <remarks>
        /// The values of latitude and longitude will be wrapped to already remain within
        /// 90 degrees of positive or negative latitude, and 180 degrees of positive or negative longitude.
        /// </remarks>
        /// <returns>The rotated polar coordinate.</returns>
        /// <param name="polar">The polar coordinate to rotate..</param>
        /// <param name="lat">Degrees of latitude to rotate by.</param>
        /// <param name="lon">Degrees of longitude to rotate by.</param>
        public static Vector2 PolarRotate(this Vector2 polar, float lat, float lon)
        {
            var newLat = polar.x + lat;
            var newLon = polar.y + lon;

            if (newLat > 90f)
                newLat -= 180f;
            else if (newLat < -90)
                newLat += 180f;

            if (newLon > 180f)
                newLon -= 360f;
            else if (newLon < -180f)
                newLon += 360f;

            return new Vector2(newLat, newLon);
        }

        /// <summary>
        /// Clamps the latitude of a polar coordinate to an allowed range.
        /// </summary>
        /// <returns>The clamped polar coordinates.</returns>
        /// <param name="polar">The polar coordinate to clamp the latitude of.</param>
        /// <param name="min">Minimum latitude.</param>
        /// <param name="max">Maximum latitude.</param>
        public static Vector2 ClampLatitude(this Vector2 polar, float min, float max)
        {
            Assert.IsTrue(min < max, "Invalid latitude clamp range. The minimum latitude must be less than the maximum latitude");

            if (polar.x > max)
                polar.Set(max, polar.y);
            else if (polar.x < min)
                polar.Set(min, polar.y);

            return polar;
        }

        /// <summary>
        /// Clamps a point in screen space to the boundary of a circle.
        /// </summary>
        /// <remarks>
        /// Useful for displaying indicators in a circle around an object.
        /// </remarks>
        /// <returns>The new position on the circle.</returns>
        /// <param name="screenPos">The screen position to clamp to the circle.</param>
        /// <param name="screenCentre">The screen position of the origin of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        public static Vector2 ClampScreenPosToCircle(this Vector2 screenPos, Vector2 origin, float radius)
        {
            return origin + ((screenPos - origin).normalized * radius);
        }
    }
}
