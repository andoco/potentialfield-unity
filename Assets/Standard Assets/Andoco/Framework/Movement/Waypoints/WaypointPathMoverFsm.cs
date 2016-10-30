namespace Andoco.Unity.Framework.Movement.Waypoints
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Andoco.Core;

    public class WaypointPathMoverFsm : SimpleFsm<WaypointPathMoverFsm.FsmStateKind, WaypointPathMoverFsm.FsmEventKind>
    {
        public WaypointPathMoverFsm()
        {
            this.SetupFsm();
        }

        public Action StartMovement { get; set; }

        public Action StopMovement { get; set; }

        public Func<bool> ShouldStartMovement { get; set; }

        public Action RecalculateMovement { get; set; }

        public void TriggerWaypointAdded()
        {
            this.Trigger(FsmEventKind.WaypointAdded, true);
        }

        public void TriggerRecalculate()
        {
            this.Trigger(FsmEventKind.Recalculate, true);
        }

        public void TriggerArrived()
        {
            this.Trigger(FsmEventKind.Arrived, true);
        }

        private void SetupFsm()
        {
            this.AddTransition(FsmStateKind.Idle, FsmStateKind.EvaluatingPath, FsmEventKind.WaypointAdded);
            this.AddTransition(FsmStateKind.Idle, FsmStateKind.Stopped, FsmEventKind.Stop);

            this.AddTransition(FsmStateKind.EvaluatingPath, FsmStateKind.Idle, FsmEventKind.InvalidPath);
            this.AddTransition(FsmStateKind.EvaluatingPath, FsmStateKind.Moving, FsmEventKind.Move);

            this.AddTransition(FsmStateKind.Moving, FsmStateKind.EvaluatingPath, FsmEventKind.Arrived);
            this.AddTransition(FsmStateKind.Moving, FsmStateKind.RecalculatingPath, FsmEventKind.Recalculate);
            this.AddTransition(FsmStateKind.Moving, FsmStateKind.Stopped, FsmEventKind.Stop);

            this.AddTransition(FsmStateKind.RecalculatingPath, FsmStateKind.Moving, FsmEventKind.Move);

            this.AddTransition(FsmStateKind.Stopped, FsmStateKind.Idle, FsmEventKind.StopComplete);
            // TODO: transition from Stopped to Idle?

            this.AutoWire(this);
            this.Start(FsmStateKind.Idle);
            this.Execute();
        }

        [SimpleFsm]
        protected FsmEventKind OnEvaluatingPathExecute()
        {
            if (this.ShouldStartMovement())
            {
                return FsmEventKind.Move;
            }

            return FsmEventKind.InvalidPath;
        }

        [SimpleFsm]
        protected void OnMovingEnter()
        {
            this.StartMovement();
//            this.RaiseWaypointChanged(this.Previous, this.Current);
        }

        [SimpleFsm]
        protected FsmEventKind OnRecalculatingPathExecute()
        {
            this.RecalculateMovement();

            return FsmEventKind.Move;
        }

        [SimpleFsm]
        protected FsmEventKind OnStoppedExecute()
        {
            this.StopMovement();

            return FsmEventKind.StopComplete;
        }

        public enum FsmStateKind
        {
            None,
            Idle,
            EvaluatingPath,
            CalculatingPath,
            Moving,
            RecalculatingPath,
            Stopped
        }

        public enum FsmEventKind
        {
            None,
            Arrived,
            InvalidPath,
            Move,
            WaypointAdded,
            Recalculate,
            Stop,
            StopComplete,
            ValidPath
        }
    }
}
