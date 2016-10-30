namespace Andoco.Unity.Framework.Core
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Clamps a float field between <see cref="Min"/> and <see cref="Max"/>.
    /// </summary>
    [Serializable]
    public struct ClampedFloat
    {
        [SerializeField]
        private float min;

        [SerializeField]
        private float max;

        [SerializeField]
        private float value;

        /// <summary>
        /// Gets or sets the current value, which will be automatically clamped.
        /// </summary>
        /// <value>The value.</value>
        public float Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = Mathf.Clamp(value, this.min, this.max);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current value is the same as the maximum value.
        /// </summary>
        /// <value><c>true</c> if this instance is max; otherwise, <c>false</c>.</value>
        public bool IsMax
        {
            get
            {
                return this.value == this.max;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current value is the same as the minimum value.
        /// </summary>
        /// <value><c>true</c> if this instance is minimum; otherwise, <c>false</c>.</value>
        public bool IsMin
        {
            get
            {
                return this.value == this.min;
            }
        }

        /// <summary>
        /// Gets the minimum allowed value in the clamped range.
        /// </summary>
        public float Min { get { return this.min; } }

        /// <summary>
        /// Gets the maximum allowed value in the clamped range.
        /// </summary>
        public float Max { get { return this.max; } }

        /// <summary>
        /// Configure the specified min and max as the permitted clamped range.
        /// </summary>
        /// <param name="min">Minimum allowed value.</param>
        /// <param name="max">Maximum allowed value.</param>
        public void Configure(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
