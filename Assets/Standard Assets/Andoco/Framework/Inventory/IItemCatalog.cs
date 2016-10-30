namespace Andoco.Unity.Framework.Inventory
{
    /// <summary>
    /// Provides information on available items.
    /// </summary>
    public interface IItemCatalog
    {
        CatalogItem this[int index] { get; }

        CatalogItem this[string name] { get; }

        int Count { get; }

        void LoadDataset(string datasetKey);

        void Add(CatalogItem item);

        void Clear();

        CatalogItem Find(string name);

        bool TryFind(string name, out CatalogItem item);
    }
}