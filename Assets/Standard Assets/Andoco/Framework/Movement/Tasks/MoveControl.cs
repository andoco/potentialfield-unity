using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using Andoco.Unity.Framework.Movement.Movers;
using UnityEngine;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Movement.Tasks
{
    public class MoveControl : ActionTask
    {
        public MoveControl(ITaskIdBuilder id)
            : base(id)
        {
        }

        public ActionKind Action { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var mover = node.Context.GetGameObject().GetComponent<Mover>();

            switch (this.Action)
            {
                case ActionKind.None:
                    break;
                case ActionKind.Play:
                    mover.Driver.Play();
                    break;
                case ActionKind.Pause:
                    mover.Driver.Pause();
                    break;
                case ActionKind.Cancel:
                    mover.Driver.Cancel();
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown move control action {0}", this.Action));
            }

            return TaskResult.Success;
        }

        public enum ActionKind
        {
            None,
            Play,
            Pause,
            Cancel
        }
    }
}
