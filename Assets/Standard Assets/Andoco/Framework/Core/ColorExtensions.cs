namespace Andoco.Unity.Framework.Core
{
	using UnityEngine;

    public static class ColorExtensions
    {
        /// <summary>
        /// Returns the same color with a new alpha value.
        /// </summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }
    }
}
