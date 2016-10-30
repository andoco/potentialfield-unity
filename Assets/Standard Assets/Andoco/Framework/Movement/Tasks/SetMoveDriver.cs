using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using Andoco.Unity.Framework.Movement.Movers;
using UnityEngine;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Movement.Tasks
{
	[Obsolete("Use SetMover instead")]
    public class SetMoveDriver : ActionTask
    {
        public SetMoveDriver(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Driver { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            Assert.IsFalse(string.IsNullOrEmpty(this.Driver));

            var mover = node.Context.GetGameObject().GetComponent<Mover>();

            mover.SetDriver(this.Driver);

            return TaskResult.Success;
        }
    }
}
