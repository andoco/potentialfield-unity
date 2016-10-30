namespace Andoco.Unity.Framework.Movement.Movers
{
    using UnityEngine;
    using System.Collections;
    using Andoco.Unity.Framework.Movement.Objectives;

    public class LoggingMoveModifier : MonoBehaviour, IMoveModifier
    {
        public void StartModifying(IMoveDriver moveDriver)
        {
            moveDriver.ObjectiveChanged += this.OnObjectiveChanged;
        }

        public void StopModifying(IMoveDriver moveDriver)
        {
            moveDriver.ObjectiveChanged -= this.OnObjectiveChanged;
        }

        private void OnObjectiveChanged(IMoveDriver driver, IObjective previousObjective, IObjective currentObjective)
        {
            Debug.LogFormat("Previous objective: {0}, current objective {1}", previousObjective, currentObjective);
        }
    }
}