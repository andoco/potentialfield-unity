namespace Andoco.Unity.Framework.Inventory
{
    public interface IInventory
    {
        InventoryItem this[int index] { get; }
        
        int Count { get; }

        int Add(CatalogItemRef catalogItem, int amount);

        int Check(CatalogItemRef catalogItem);

        void Clear();

        InventoryItem Find(CatalogItemRef catalogItem);

        int Take(CatalogItemRef catalogItem, int amount);

        bool TryFind(CatalogItemRef catalogItem, out InventoryItem item);
    }
}