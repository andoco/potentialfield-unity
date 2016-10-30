using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Core.Pooling;
using Andoco.Unity.Framework.BehaviorTree;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Core.Scene.Management;
using Andoco.Unity.Framework.Misc;

namespace Andoco.Unity.Framework.Inventory.Tasks
{
    public class CollectItem : ActionTask
    {
        readonly IComponentIndex compIndex;

        readonly IGameObjectManager goMgr;

        public CollectItem(ITaskIdBuilder id, IComponentIndex compIndex, IGameObjectManager goMgr)
            : base(id)
        {
            this.goMgr = goMgr;
            this.compIndex = compIndex;
        }

        public bool SharedInventory { get; set; }

        public string Group { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            var go = node.Context.GetGameObject();
            var groups = go.GetComponent<GameObjectGroup>();
            var inventory = SharedInventory ? compIndex.GetSingle<IInventory>() : go.GetComponent<IInventory>();
            var items = groups.GetInGroup(Group);

            for (int i = 0; i < items.Count; i++)
            {
                var collectible = items[i].GetComponentInEntity<CollectibleItem>();
                inventory.Add(collectible.catalogItem, collectible.quantity);

                goMgr.Destroy(collectible.gameObject);
            }

            items.ReturnToPool();

            return TaskResult.Success;
        }
    }
}
