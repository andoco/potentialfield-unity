namespace Andoco.Unity.Framework.Inventory
{
    using UnityEngine;

    public interface ICatalogItemHandler
    {
        void ActivateItem(CatalogItemRef item, GameObject owner);
    }
}