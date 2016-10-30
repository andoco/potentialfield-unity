namespace Andoco.Unity.Framework.Movement.Movers.Drivers
{
    using UnityEngine;
    using Andoco.Unity.Framework.Movement.Objectives;

    public class NullMoveDriver : IMoveDriver
    {
        public static NullMoveDriver Instance = new NullMoveDriver();

        public NullMoveDriver()
        {
            this.ObjectiveChanged = null;
            this.StateChanged = null;
        }

        public event ObjectiveChangedDelegate ObjectiveChanged;

        public event MoverStateChangedDelegate StateChanged;

        public string driverName;

        public string Name
        {
            get
            {
                return this.driverName;
            }
        }

        public float Speed { get; set; }

        public MoverStateKind CurrentState { get { return MoverStateKind.None; } }

        public void SetActiveDriver(bool isActive)
        {
        }

        public void Cancel()
        {
        }

        public void MoveTo(IObjective destination)
        {
        }

        public void EnqueueMoveTo(IObjective destination)
        {
        }

        public void Pause()
        {
        }

        public void Play()
        {
        }

        public void Clear()
        {
        }
    }
}