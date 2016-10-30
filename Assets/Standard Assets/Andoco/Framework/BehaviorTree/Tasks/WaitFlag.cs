using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.BehaviorTree.Scheduler;
using Andoco.BehaviorTree.Signals;
using Andoco.Core.Signals;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine;

namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
    public class WaitFlag : ActionTask
    {
        private readonly ITaskScheduler scheduler;
    
        private readonly FlagSignal flagSignal;

        public WaitFlag(ITaskIdBuilder id, ITaskScheduler scheduler, FlagSignal flagSignal)
            : base(id)
        {
            this.flagSignal = flagSignal;
            this.scheduler = scheduler;
        }

        public string Flag { get; set; }

        protected override TaskState CreateState()
        {
            return new State();
        }

        public override TaskResult Run(ITaskNode node)
        {
            var state = (State)node.State;
    
            if (state.slot == null)
            {
                var sigCtx = node.Context.GetSignalContext();
    
                state.slot = flagSignal.AddListener(sigCtx, data =>
                {
                    if (string.Equals(data.name, this.Flag, System.StringComparison.OrdinalIgnoreCase))
                    {
                        this.scheduler.Schedule(node.Context);
    
                        // We only want to process one raising of the flag, so pause until the task is re-run.
                        state.slot.Pause();
    
                        // Indicate that we received the flag, so we can return Success on the next iteration.
                        state.received = true;
                        state.waiting = false;
                    }
                });
            }
    
            if (state.received)
            {
                state.received = false;
                return TaskResult.Success;
            }
            else if (state.waiting)
            {
                return TaskResult.Pending;
            }
            else
            {
                // Resume for the signal slot so that we receive the next raising of the slot.
                if (state.slot.IsPaused)
                    state.slot.Resume();
                
                state.waiting = true;
    
                return TaskResult.Pending;
            }
        }

        protected override void Stopped(ITaskNode node)
        {
            var state = (State)node.State;

            // The task can be stopped while waiting for the signal, so need to pause it.
            if (!state.slot.IsPaused)
            {
                state.slot.Pause();
            }

            state.waiting = false;
        }

        private class State : TaskState
        {
            public ISlotController slot;
            public bool waiting;
            public bool received;

            protected override void OnReset()
            {
                slot.Stop();
                slot = null;
                received = false;
            }
        }
    }
}
