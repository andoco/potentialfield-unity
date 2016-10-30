namespace Andoco.Unity.Framework.Inventory
{
    using System;

    [Serializable]
    public class InventoryItem
    {
        public CatalogItemRef catalogItem;

        public int amount;

        public int limit;
    }
}