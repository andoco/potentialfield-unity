namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;

    /// <summary>
    /// Gets a random integer value between a configurable <see cref="Min"/> and <see cref="Max"/> range.
    /// </summary>
    [System.Serializable]
    public struct RandomInt
    {
        [SerializeField]
        private int min;

        [SerializeField]
        private int max;

        /// <summary>
        /// Gets the next random value in the configured range.
        /// </summary>
        public int Value
        {
            get
            {
                return Random.Range(this.min, this.max);
            }
        }

        /// <summary>
        /// Gets the inclusive lower bound of the random range.
        /// </summary>
        public int Min { get { return this.min; } }

        /// <summary>
        /// Gets the exclusive upper bound of the random range.
        /// </summary>
        public int Max { get { return this.max; } }

        /// <summary>
        /// Configure the specified min and max as the permitted random range.
        /// </summary>
        /// <param name="min">Minimum allowed value.</param>
        /// <param name="max">Maximum allowed value.</param>
        public void Configure(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
