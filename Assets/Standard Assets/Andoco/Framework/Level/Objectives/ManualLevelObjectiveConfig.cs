namespace Andoco.Unity.Framework.Level.Objectives
{
    using System;

    [Serializable]
    public class ManualLevelObjectiveConfig : LevelObjectiveConfig
    {
        public bool isComplete;

        public bool IsComplete { get { return this.isComplete; } }
    }
}
