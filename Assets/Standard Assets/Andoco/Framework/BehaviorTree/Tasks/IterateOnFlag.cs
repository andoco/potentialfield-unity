using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.BehaviorTree.Scheduler;
using Andoco.BehaviorTree.Signals;
using Andoco.Core.Signals;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine;

namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
    public class IterateOnFlag : ActionTask
    {
        private readonly ITaskScheduler scheduler;
    
        private readonly FlagSignal flagSignal;

        public IterateOnFlag(ITaskIdBuilder id, ITaskScheduler scheduler, FlagSignal flagSignal)
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
                        // The callback can be executed even if the enclosing subtree is stopped, so we potentially schedule
                        // an unnecessary iteration here. But we allow it because it won't be harmful, and there's no way of
                        // pausing it without breaking the expected behaviour.
                        //
                        // TODO: Maybe supply a StopReason to the stop methods.
                        this.scheduler.Schedule(node.Context);
    
                        // We only want to process one raising of the flag, so pause until the task is re-run.
                        state.slot.Pause();
                    }
                });
            }
    
            // Resume for the signal slot so that we receive the next raising of the slot.
            if (state.slot.IsPaused)
                state.slot.Resume();
    
            return TaskResult.Success;
        }

        private class State : TaskState
        {
            public ISlotController slot;

            protected override void OnReset()
            {
                slot.Stop();
                slot = null;
            }
        }
    }
}
