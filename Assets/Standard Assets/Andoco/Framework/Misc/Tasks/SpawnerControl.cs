using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Misc
{
    public class SpawnerControl : ActionTask
    {
        public SpawnerControl(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Spawner { get; set; }

        public ActionKind Action { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var go = node.Context.GetGameObject();

            Spawner spawner;
            if (string.IsNullOrEmpty(this.Spawner))
            {
                spawner = go.GetComponent<Spawner>();
            }
            else
            {
                spawner = go.transform.Find(this.Spawner).GetComponent<Spawner>();
            }

            Assert.IsNotNull(spawner);

            switch (this.Action)
            {
                case ActionKind.None:
                    break;
                case ActionKind.Start:
                    spawner.Spawn();
                    break;
                case ActionKind.Stop:
                    spawner.StopSpawning();
                    break;
                case ActionKind.Unlock:
                    spawner.Unlock();
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unknown SpawnerControl action {0}", this.Action));
            }

            return TaskResult.Success;
        }

        public enum ActionKind
        {
            None,
            Start,
            Stop,
            Unlock
        }
    }
}

