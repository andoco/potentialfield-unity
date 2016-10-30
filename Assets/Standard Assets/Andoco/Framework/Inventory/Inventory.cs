namespace Andoco.Unity.Framework.Inventory
{
    using System;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using UnityEngine;
    using Zenject;

    public class Inventory : MonoBehaviour, IInventory
    {
        [Inject]
        private IStructuredLog log;

        [SerializeField]
        private List<InventoryItem> items;

        #region Lifecycle

        void Recycled()
        {
            Clear();
        }

        #endregion

        #region IInventory implementation

        public InventoryItem this[int index]
        {
            get
            {
                return this.items[index];
            }
        }
        
        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }

        public int Add(CatalogItemRef catalogItem, int amount)
        {
            InventoryItem item;
            if (!this.TryFind(catalogItem, out item))
            {
                item = new InventoryItem { catalogItem = catalogItem };
                this.items.Add(item);
            }
            
            item.amount += amount;
            
            return amount;
        }

        public int Check(CatalogItemRef catalogItem)
        {
            InventoryItem item;
            if (this.TryFind(catalogItem, out item))
            {
                return item.amount;
            }

            return 0;
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public InventoryItem Find(CatalogItemRef catalogItem)
        {
            InventoryItem item;
            if (!this.TryFind(catalogItem, out item))
            {
                throw new ArgumentException(string.Format("An item with the name {0} could not be found in the inventory", name));
            }

            return item;
        }

        public int Take(CatalogItemRef catalogItem, int amount)
        {
            InventoryItem item;
            int take = 0;

            if (this.TryFind(catalogItem, out item))
            {
                take = Math.Min(amount, item.amount);
                item.amount -= take;
            }

            return take;
        }

        public bool TryFind(CatalogItemRef catalogItem, out InventoryItem item)
        {
            for (int i=0; i < this.items.Count; i++)
            {
                if (this.items[i].catalogItem == catalogItem)
                {
                    item = this.items[i];
                    return true;
                }
            }

            item = default(InventoryItem);
            return false;
        }

        #endregion
    }
}