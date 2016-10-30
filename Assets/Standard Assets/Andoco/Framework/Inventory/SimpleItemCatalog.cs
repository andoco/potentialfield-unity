namespace Andoco.Unity.Framework.Inventory
{
    using System;
    using UnityEngine;

    public class SimpleItemCatalog : IItemCatalog
    {
        private ItemCatalogData data;

        #region IItemCatalog implementation

        public CatalogItem this[int index]
        {
            get
            {
                return this.data.items[index];
            }
        }

        public CatalogItem this[string name]
        {
            get
            {
                return this.Find(name);
            }
        }

        public int Count
        {
            get
            {
                return this.data.items.Length;
            }
        }

        public void LoadDataset(string datasetKey)
        {
            var allData = Resources.LoadAll<ItemCatalogData>("");
            var datasetIndex = Array.FindIndex(allData, x => x.datasetKey.Equals(datasetKey, StringComparison.OrdinalIgnoreCase));
            data = allData[datasetIndex];
        }

        public void Add(CatalogItem item)
        {
            throw new NotImplementedException();

            //ItemModel existingItem;

            //if (this.TryFind(item.name, out existingItem))
            //{
            //    throw new ArgumentException(string.Format("An item with the name {0} already exists in the catalog", item.name));
            //}

            //this.source.Add(item);
        }

        public void Clear()
        {
            throw new NotImplementedException();
            //this.source.Clear();
        }

        public bool TryFind(string name, out CatalogItem item)
        {
            var items = this.data.items;

            for (int i=0; i < items.Length; i++)
            {
                if (string.Equals(items[i].name, name, StringComparison.OrdinalIgnoreCase))
                {
                    item = items[i];
                    return true;
                }
            }

            item = default(CatalogItem);
            return false;
        }

        public CatalogItem Find(string name)
        {
            CatalogItem item;
            if (!this.TryFind(name, out item))
            {
                throw new ArgumentException(string.Format("An item with the name {0} could not be found in the catalog", name));
            }

            return item;
        }

        #endregion
    }
}