namespace Andoco.Unity.Framework.Movement.Tasks
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Movement.Movers;
    using UnityEngine;

    public class MoveState : TaskState
    {
        public bool willMove = true;
        public bool readyToMove;
        public Mover Mover { get; set; }

        public void ResetMover()
        {
            if (this.Mover.Driver != null)
            {
                this.Mover.Driver.Cancel();
                this.Mover.Driver.Clear();
            }
        }

        protected override void OnReset()
        {
            this.willMove = true;
            this.readyToMove = false;
            this.ResetMover();
        }
    }

    public abstract class Move : ActionTask
    {
        protected Move(ITaskIdBuilder id)
            : base(id)
        {
            this.CancellationResult = TaskResult.Failure;
        }

        public ValidationModeKind ValidationMode { get; set; }

        /// <summary>
        /// The result to return when the mover enters the <see cref="MoverStateKind.Cancelled"/> state. Default value is <see cref="TaskResult.Failure"/>.
        /// </summary>
        public TaskResult CancellationResult { get; set; }

        protected override TaskState CreateState()
        {
            return new MoveState();
        }

        protected override void Initializing(ITaskNode node)
        {
            var go = node.Context.GetGameObject();
            var state = (MoveState)node.State;
            state.Mover = go.GetComponent<Mover>();
        }

        protected override void Started(ITaskNode node)
        {
            var state = (MoveState)node.State;
            SetupMovement(state);
        }

        public override TaskResult Run(ITaskNode node)
        {
            var state = (MoveState)node.State;

            if (!state.willMove)
            {
                this.OnMoveCancelled(state);
                return this.CancellationResult;
            }

            if (!state.readyToMove)
                return TaskResult.Pending;

            switch (state.Mover.Driver.CurrentState)
            {
                case MoverStateKind.None:
                    return TaskResult.Pending;

                case MoverStateKind.Moving:
                    if (!this.OnMoveValidate(state))
                    {
                        return this.HandleValidationFailure(state);
                    }

                    return TaskResult.Pending;

                case MoverStateKind.Arrived:
                    this.OnMoveCompleted(state);
                    return TaskResult.Success;

                case MoverStateKind.Cancelled:
                    this.OnMoveCancelled(state);
                    return this.CancellationResult;

                default:
                    throw new System.InvalidOperationException(string.Format("Unexpected {0} value {1}", typeof(MoverStateKind), state.Mover.Driver.CurrentState));
            }
        }

        protected override void Stopped(ITaskNode node)
        {
            var state = (MoveState)node.State;
            state.ResetMover();
        }

        #region Template methods

        protected virtual bool TryFindMoveTarget(MoveState state, out Vector3 destination)
        {
            destination = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Called when movement needs to be prepared, usually by picking a destination or creating a path of waypoints.
        /// </summary>
        /// <param name="state">The move state.</param>
        protected virtual void OnPrepareMovement(MoveState state)
        {
            Vector3 destination;
            if (this.TryFindMoveTarget(state, out destination))
            {
                // TODO: Probably need a more reliable way to indicate that movment is unnecessary, to avoid floating
                // point precision issues. E.g. WillMove() template method.
                if (destination == state.Mover.transform.position)
                {
                    state.willMove = false;
                }
                else
                {
                    state.willMove = true;
                    state.Mover.Driver.MoveTo(destination);
                }
            }
        }

        /// <summary>
        /// Called after a destination has been picked and movement should start.
        /// </summary>
        /// <remarks>
        /// This method should be overloaded to setup movement based on the picked destination. For instance, adding
        /// waypoints from the current position to the destination.
        /// </remarks>
        /// <param name="state">State.</param>
        protected virtual void OnMoveStarted(MoveState state)
        {
        }

        protected virtual bool OnMoveValidate(MoveState state)
        {
            return true;
        }

        protected virtual void OnMoveCompleted(MoveState state)
        {
        }

        protected virtual void OnMoveCancelled(MoveState state)
        {
        }

        #endregion

        #region Private methods

        private void SetupMovement(MoveState state)
        {
            state.willMove = true;

            this.OnPrepareMovement(state);

            if (state.willMove)
            {
                this.OnMoveStarted(state);
            }
        }

        private TaskResult HandleValidationFailure(MoveState state)
        {
            switch (this.ValidationMode)
            {
            case ValidationModeKind.Fail:
                return TaskResult.Failure;
            case ValidationModeKind.Ignore:
                return TaskResult.Pending;
            case ValidationModeKind.Restart:
                state.Mover.Driver.Cancel();
                //state.Mover.Driver.Clear();
                this.SetupMovement(state);
                return TaskResult.Pending;
            default:
                throw new System.InvalidOperationException(string.Format("Unexpected {0} value {1}", typeof(MoverStateKind), this.ValidationMode));
            }
        }

        #endregion

        #region Types

        public enum ValidationModeKind
        {
            None,
            Fail,
            Ignore,
            Restart
        }

        #endregion
    }
}
