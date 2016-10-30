using Zenject;
using UnityEngine;
using System;

namespace Andoco.Unity.Framework.Level.Objectives
{
    public class LevelFinishedLevelObjective : MonoBehaviour, ILevelObjective
    {
        [Inject]
        private ILevelSystem level;

        public LevelFinishedLevelObjectiveConfig config;

        public string Name { get { return this.config.Name; } }

        public string Category { get { return this.config.Category; } }

        public bool IsRequired { get { return this.config.IsRequired; } }

        public LevelObjectiveResult CheckObjectiveStatus()
        {
            return this.level.LevelNumber == this.config.levelNumber
                ? LevelObjectiveResult.Completed
                : LevelObjectiveResult.Pending;
        }

        public void Reset()
        {
        }
    }
}
