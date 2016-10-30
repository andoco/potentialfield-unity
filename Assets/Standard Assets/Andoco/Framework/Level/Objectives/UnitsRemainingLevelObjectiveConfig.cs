namespace Andoco.Unity.Framework.Level.Objectives
{
    using System;
    using UnityEngine;

    [Serializable]
    public class UnitsRemainingLevelObjectiveConfig : LevelObjectiveConfig
    {
        public GameObject unitPrefab;

        public UnitsRemainingLevelObjectiveMode comparisonMode;

        public int comparisonValue;

        [Tooltip("The number of units that must be detected in the scene before the level objective is activated")]
        public int activationValue;
    }
}
