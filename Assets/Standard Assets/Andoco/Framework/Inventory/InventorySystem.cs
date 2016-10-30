namespace Andoco.Unity.Framework.Inventory
{
    using UnityEngine;
    using Zenject;

    public class InventorySystem : MonoBehaviour, IInventorySystem
    {
        [Inject]
        IItemCatalog catalog;

        public string datasetKey = "Default";
        public ItemHandlerEntry[] itemHandlers;

        public IItemCatalog Catalog { get { return catalog; } }

        [Inject]
        void OnPostInject()
        {
            catalog.LoadDataset(datasetKey);
        }

        public void ActivateItem(CatalogItemRef item, GameObject owner)
        {
            for (int i = 0; i < itemHandlers.Length; i++)
            {
                var entry = itemHandlers[i];
                if (entry.enabled && entry.catalogItem == item)
                {
                    var handler = entry.itemHandler as ICatalogItemHandler;

                    if (handler != null)
                    {
                        handler.ActivateItem(item, owner);
                    }
                }
            }
        }

        [System.Serializable]
        public class ItemHandlerEntry
        {
            public CatalogItemRef catalogItem;
            public Component itemHandler;
            public bool enabled;
        }
    }
}