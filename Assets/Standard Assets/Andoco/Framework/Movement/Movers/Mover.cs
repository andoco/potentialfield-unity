namespace Andoco.Unity.Framework.Movement.Movers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.BehaviorTree.Signals;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core;
    using Andoco.Unity.Framework.Core.DiagnosticsIndicators;
    using Andoco.Unity.Framework.Movement.Movers.Drivers;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public class Mover : MonoBehaviour
    {
        private const string StateDiagnosticsKey = "MoveState";

        private IMoveModifier[] moveModifiers;
        private string originalMoveDriverName;

        [Inject]
        private IStructuredLog log;

        [Inject]
        private DiagnosticIndicatorSignal diagnosticSignal;

        [Inject]
        private FlagSignal flagSignal;

        #region Fields

        public string moveDriverName;
        public bool dispatchDiagnosticIndicatorSignals;
        public bool raiseFlagOnArrival;
        public string arrivedFlag = "arrived";
        public bool pauseOnStart;

        #endregion

        #region Events

        public event Action<IMoveDriver> Arrived;

        public event Action<IMoveDriver> Cancelled;

        #endregion

        #region Properties

        public IMoveDriver Driver { get; private set; }

        public IMoveNavigator Navigator { get; private set; }

        #endregion

        #region Lifecycle

        void Awake()
        {
            this.moveModifiers = this.gameObject.GetComponentsInChildren<IMoveModifier>();
            this.originalMoveDriverName = this.moveDriverName;
        }

        void Start()
        {
            this.SetDriver(this.moveDriverName);
            this.CheckInitialPauseState();
        }

        void Spawned()
        {
            this.CheckInitialPauseState();
        }

        void Recycled()
        {
            this.moveDriverName = this.originalMoveDriverName;

            if (!this.Driver.Name.Equals(this.moveDriverName, StringComparison.OrdinalIgnoreCase))
            {
                this.SetDriver(this.moveDriverName);
            }
        }

        #endregion

        #region Public methods

        public IEnumerable<IMoveDriver> GetDrivers()
        {
            return this.gameObject.GetComponentsInChildren(typeof(IMoveDriver), true).Cast<IMoveDriver>();
        }

        public bool IsCurrentDriver(string name)
        {
            Assert.IsFalse(string.IsNullOrEmpty(name));

            if (this.Driver == null)
                return false;
            
            return string.Equals(this.Driver.Name, name, StringComparison.OrdinalIgnoreCase);
        }

        public void SetDriver(string name)
        {
            this.log.Trace("Setting move driver").Field("driver", name).Write();

            if (this.IsCurrentDriver(name))
                return;

            if (this.Driver != null)
            {
                this.Driver.Cancel();
                this.Driver.Clear();
                this.Driver.StateChanged -= this.OnMoverStateChanged;

                for (var i=0; i < this.moveModifiers.Length; i++)
                {
                    this.moveModifiers[i].StopModifying(this.Driver);
                }

                this.Driver.SetActiveDriver(false);
            }

            this.Driver = this.GetDriverOrDefault(name) ?? NullMoveDriver.Instance;
            this.moveDriverName = this.Driver.Name;
            this.Driver.StateChanged += this.OnMoverStateChanged;

            for (var i=0; i < this.moveModifiers.Length; i++)
            {
                this.moveModifiers[i].StartModifying(this.Driver);
            }

            this.Driver.SetActiveDriver(true);
        }

        public void SetNavigator(string name)
        {
            if (this.Navigator != null)
            {
                this.Navigator.StopNavigating();
            }

            var allNavigators = this.GetComponentsInChildren<IMoveNavigator>(true);
            var navigator = allNavigators.SingleOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (navigator == null)
                throw new ArgumentOutOfRangeException("name", name, "A matching navigator could not be found");

            this.Navigator = navigator;
            this.Navigator.StartNavigating();
        }

        #endregion

        #region Private methods

        private void CheckInitialPauseState()
        {
            if (!this.pauseOnStart)
                return;

            this.Driver.Pause();
        }

        private IMoveDriver GetDriverOrDefault(string name)
        {
            return this.GetDrivers().SingleOrDefault(x => x.Name == name);
        }

        private void OnMoverStateChanged(IMoveDriver driver, MoverStateKind previousState, MoverStateKind currentState)
        {
            Assert.AreEqual(this.Driver, driver);

            if (this.dispatchDiagnosticIndicatorSignals)
            {
                this.diagnosticSignal.DispatchSet(
                    this.gameObject, 
                    StateDiagnosticsKey, 
                    currentState.ToString());
            }

            switch (currentState)
            {
                case MoverStateKind.Arrived:
                    this.log.Trace("Arrived").Write();

                    if (this.Arrived != null)
                        this.Arrived(driver);
                    
                    if (this.raiseFlagOnArrival)
                    {
                        this.log.Trace("Raising arrival flag").Field("flag", this.arrivedFlag).Write();
                        this.flagSignal.DispatchFlag(this.gameObject, this.arrivedFlag);
                    }
                    
                    break;
                case MoverStateKind.Cancelled:
                    this.log.Trace("Cancelled").Write();

                    if (this.Cancelled != null)
                        this.Cancelled(driver);
                    break;
            }
        }

        #endregion
    }
}