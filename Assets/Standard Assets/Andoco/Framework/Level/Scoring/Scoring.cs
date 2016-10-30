namespace Andoco.Unity.Framework.Level.Scoring
{
    using UnityEngine;
    using Zenject;
    using Andoco.Unity.Framework.Core;

    public class Scoring : MonoBehaviour
    {
//        [Inject]
        private ScoreSystem system;

        public PointConf[] points;

        [Inject]
        void OnPostInject()
        {
            this.system = Indexed.GetSingle<ScoreSystem>();
        }

        public void AddPoints(string key)
        {
            float amount = 0f;

            for (int i = 0; i < points.Length; i++)
            {
                var p = this.points[i];

                if (string.Equals(key, p.key, System.StringComparison.OrdinalIgnoreCase))
                {
                    amount = p.points;
                    break;
                }
            }

            this.system.AddPoints(amount);
        }

        [System.Serializable]
        public class PointConf
        {
            public string key;
            public float points;
        }
    }
}
