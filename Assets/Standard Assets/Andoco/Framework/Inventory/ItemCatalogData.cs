namespace Andoco.Unity.Framework.Inventory
{
    using UnityEngine;

    [System.Serializable]
    public class ItemCatalogData : ScriptableObject
    {
        public string datasetKey = "Default";

        public CatalogItem[] items;
    }
}