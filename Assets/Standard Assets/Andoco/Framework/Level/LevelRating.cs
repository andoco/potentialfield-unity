namespace Andoco.Unity.Framework.Level
{
    using UnityEngine;
    using Zenject;
    using Andoco.Unity.Framework.Core;

    public class LevelRating : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The level number at which the rating will be at its maximum")]
        private int upperRatingLevel = 10;

        public LevelSystem level;

        public float RatingFactor { get; private set; }

        public int UpperRatingLevel { get { return this.upperRatingLevel; } }

        public void Calculate()
        {
            var startRating = this.RatingFactor;

            // Increase linearly with levelNumber.
            this.RatingFactor = Mathf.Min(1f / (this.UpperRatingLevel - 1) * (this.level.LevelNumber - 1), 1f);

            Debug.LogFormat("Increased level rating from {0} to {1}", startRating, this.RatingFactor);
        }

        public float CalculateLinear(FloatRange rating)
        {
            var range = rating.Max - rating.Min;
            var max = rating.Min + (this.RatingFactor * range);

            return max;
        }
    }
}
