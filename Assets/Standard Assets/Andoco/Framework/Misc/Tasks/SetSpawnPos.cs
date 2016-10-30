using System;
using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using UnityEngine;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Misc
{
    public class SetSpawnPos : ActionTask
    {
        public SetSpawnPos(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Key { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var spawner = node.Context.GetComponent<Spawner>();
            var pos = node.Context.GetItem<Vector3>(Key);

            spawner.SpawnPosition = pos;

            return TaskResult.Success;
        }
    }
}
