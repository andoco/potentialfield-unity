namespace Andoco.Unity.Framework.Movement.Movers
{
    using UnityEngine;
    using Andoco.Unity.Framework.Movement.Objectives;

    public static class MoveDriverExtensions
    {
        public static void MoveTo(this IMoveDriver moveDriver, Vector3 destination)
        {
            moveDriver.MoveTo(new Objective(destination));
        }

        public static void EnqueueMoveTo(this IMoveDriver moveDriver, Vector3 destination)
        {
            moveDriver.EnqueueMoveTo(new Objective(destination));
        }
    }
}