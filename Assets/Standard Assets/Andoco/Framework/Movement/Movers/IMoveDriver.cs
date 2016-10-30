namespace Andoco.Unity.Framework.Movement.Movers
{
    using UnityEngine;
    using System.Collections;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Movement.Objectives;

    public delegate void ObjectiveChangedDelegate(IMoveDriver driver, IObjective previousObjective, IObjective currentObjective);

    public delegate void MoverStateChangedDelegate(IMoveDriver driver, MoverStateKind previousState, MoverStateKind currentState);

    public interface IMoveDriver : INamedConfig
    {
        #region Events

        event ObjectiveChangedDelegate ObjectiveChanged;

        event MoverStateChangedDelegate StateChanged;

        #endregion

        #region Properties

        float Speed { get; set; }

        MoverStateKind CurrentState { get; }

        #endregion

        #region Methods

        void SetActiveDriver(bool isActive);

        void Cancel();

        void MoveTo(IObjective destination);

        void EnqueueMoveTo(IObjective destination);

        void Pause();

        void Play();

        void Clear();

        #endregion
    }
}