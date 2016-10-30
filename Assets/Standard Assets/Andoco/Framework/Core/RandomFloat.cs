﻿namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

    /// <summary>
    /// Gets a random float value between a configurable <see cref="Min"/> and <see cref="Max"/> range.
    /// </summary>
    [System.Serializable]
    public struct RandomFloat
    {
        [SerializeField]
        private float min;

        [SerializeField]
        private float max;

        /// <summary>
        /// Gets the next random value in the configured range.
        /// </summary>
        public float Value
        {
            get
            {
                return Random.Range(this.min, this.max);
            }
        }

        /// <summary>
        /// Gets the inclusive lower bound of the random range.
        /// </summary>
        public float Min { get { return this.min; } }

        /// <summary>
        /// Gets the inclusive upper bound of the random range.
        /// </summary>
        public float Max { get { return this.max; } }

        /// <summary>
        /// Configure the specified min and max as the permitted random range.
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
