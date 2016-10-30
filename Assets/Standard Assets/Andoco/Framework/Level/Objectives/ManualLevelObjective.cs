using UnityEngine;
using System;

namespace Andoco.Unity.Framework.Level.Objectives
{
    public class ManualLevelObjective : MonoBehaviour, ILevelObjective
    {
        private bool initialIsComplete;

        public ManualLevelObjectiveConfig config;

        public string Name { get { return this.config.Name; } }

        public string Category { get { return this.config.Category; } }

        public bool IsRequired { get { return this.config.IsRequired; } }
    
        void Awake()
        {
            this.initialIsComplete = this.config.isComplete;
        }
    
        public LevelObjectiveResult CheckObjectiveStatus()
        {
            return this.config.isComplete
                ? LevelObjectiveResult.Completed
                : LevelObjectiveResult.Pending;
        }

        public void Reset()
        {
            this.config.isComplete = this.initialIsComplete;
        }
    }
}
