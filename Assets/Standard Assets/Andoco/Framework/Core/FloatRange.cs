namespace Andoco.Unity.Framework.Core
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Represents the lower and upper values of a range.
    /// </summary>
    [Serializable]
    public struct FloatRange
    {
        [SerializeField]
        private float min;

        [SerializeField]
        private float max;

        /// <summary>
        /// Gets the lower value in the range.
        /// </summary>
        public float Min { get { return this.min; } }

        /// <summary>
        /// Gets the upper value in the range.
        /// </summary>
        public float Max { get { return this.max; } }

        /// <summary>
        /// Gets the difference in value between <see cref="Min"/> and <see cref="Max"/>.
        /// </summary>
        public float Spread { get { return this.Max - this.Min; } }

        /// <summary>
        /// Configure the specified min and max values of the range.
        /// </summary>
        /// <param name="min">Minimum value.</param>
        /// <param name="max">Maximum value.</param>
        public void Configure(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Clamp the value to within the configured range.
        /// </summary>
        public float Clamp(float value)
        {
            return Mathf.Clamp(value, this.Min, this.Max);
        }

        /// <summary>
        /// Normalize the value to the range 0..1, relative to the configured range.
        /// </summary>
        /// <param name="value">Value.</param>
        public float Normalize(float value)
        {
            return 1f / this.Spread * this.Clamp(value);
        }

        public override string ToString()
        {
            return string.Format("[FloatRange: Min={0}, Max={1}]", Min, Max);
        }
    }
}
