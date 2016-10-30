namespace Andoco.Unity.Framework.Misc
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Conditions;
    using Andoco.Unity.Framework.BehaviorTree;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class CheckGameObjectGroup : Condition
    {
        public CheckGameObjectGroup(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Group { get; set; }

        public StateKind State { get; set; }

        protected override bool Evaluate(ITaskNode node)
        {
            var groups = node.Context.GetGameObject().GetComponent<GameObjectGroup>();
            Assert.IsNotNull(groups);

            switch (this.State)
            {
                case StateKind.Empty:
                    return groups.IsEmpty(this.Group);
                case StateKind.Any:
                    return !groups.IsEmpty(this.Group);
                default:
                    throw new InvalidOperationException(string.Format("Unsupported {0} value {1}", typeof(StateKind), this.State));
            }
        }

        public enum StateKind
        {
            None,
            Empty,
            Any
        }
    }
}

