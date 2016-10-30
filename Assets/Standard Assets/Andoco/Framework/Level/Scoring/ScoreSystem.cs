using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Data;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.Level.Scoring
{
    public class ScoreSystem : MonoBehaviour
    {
        [Inject]
        private IGameData gameData;

        private ScoreData scoreData;

        public string scoreDataKey = "Score";

        [Inject]
        void OnPostInject()
        {
            this.scoreData = this.gameData.GetOrAdd<ScoreData>(this.scoreDataKey);
        }

        public void AddPoints(float points)
        {
            this.scoreData.Score += points;
        }

        public float GetCurrentScore()
        {
            return this.scoreData.Score;
        }

        public void ResetScore()
        {
            this.scoreData.Score = 0f;
        }
    }
}
