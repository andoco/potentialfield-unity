namespace Andoco.Unity.Framework.Inventory
{
    using UnityEngine;

    public interface IInventorySystem
    {
        IItemCatalog Catalog { get; }

        void ActivateItem(CatalogItemRef item, GameObject owner);
    }
}