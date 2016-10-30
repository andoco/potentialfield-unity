using UnityEngine;
using System.Linq;
using Andoco.Unity.Framework.Units;

namespace Andoco.Unity.Framework.Level.Objectives
{
    public class UnitsRemainingLevelObjective : MonoBehaviour, ILevelObjective
    {
        private bool isActivated;

        public UnitsRemainingLevelObjectiveConfig config;

        public string Name { get { return this.config.Name; } }

        public string Category { get { return this.config.Category; } }

        public bool IsRequired { get { return this.config.IsRequired; } }

        public LevelObjectiveResult CheckObjectiveStatus()
        {
            var units = FindObjectsOfType<Unit>();
            var remaining = units.Count(x => x.tag == this.config.unitPrefab.tag);
            var comparisonValue = this.config.comparisonValue;

            if (!this.isActivated && remaining < this.config.activationValue)
            {
                return LevelObjectiveResult.Pending;
            }

            this.isActivated = true;

            LevelObjectiveResult result;

            switch (this.config.comparisonMode)
            {
                case UnitsRemainingLevelObjectiveMode.AtLeast:
                    result = remaining >= comparisonValue ? LevelObjectiveResult.Completed : LevelObjectiveResult.Failed;
                    break;
                case UnitsRemainingLevelObjectiveMode.Exactly:
                    result = remaining == comparisonValue ? LevelObjectiveResult.Completed : LevelObjectiveResult.Failed;
                    break;
                default:
                    throw new System.InvalidOperationException(string.Format("Unknown {0} value {1}", typeof(UnitsRemainingLevelObjectiveMode), this.config.comparisonMode));
            }

            UnityEngine.Debug.LogFormat("{0} {1} for {2} = {3}", this.config.comparisonMode, comparisonValue, this.config.unitPrefab, result);

            return result;
        }

        public void Reset()
        {
            this.isActivated = false;
        }
    }

}
