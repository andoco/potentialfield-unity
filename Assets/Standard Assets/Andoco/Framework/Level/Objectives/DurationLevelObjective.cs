using Zenject;
using UnityEngine;

namespace Andoco.Unity.Framework.Level.Objectives
{
    public class DurationLevelObjective : MonoBehaviour, ILevelObjective
    {
        [Inject]
        private ILevelSystem level;

        public DurationLevelObjectiveConfig config;

        public string Name { get { return this.config.Name; } }

        public string Category { get { return this.config.Category; } }

        public bool IsRequired { get { return this.config.IsRequired; } }

        public LevelObjectiveResult CheckObjectiveStatus()
        {
            return this.level.LevelTime >= this.config.duration
                ? LevelObjectiveResult.Completed
                : LevelObjectiveResult.Pending;
        }

        public void Reset()
        {
        }
    }
}
