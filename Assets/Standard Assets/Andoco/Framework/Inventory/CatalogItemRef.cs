namespace Andoco.Unity.Framework.Inventory
{
    using System;

    [Serializable]
    public struct CatalogItemRef
    {
        public string datasetKey;

        public string catalogItemName;

        public static implicit operator string(CatalogItemRef itemRef)
        {
            return itemRef.catalogItemName;
        }

        public override bool Equals(object obj)
        {
            var other = (CatalogItemRef)obj;
            return datasetKey.Equals(other.datasetKey) && catalogItemName.Equals(other.catalogItemName);
        }

        public static bool operator ==(CatalogItemRef a, CatalogItemRef b)
        {
            return a.datasetKey.Equals(b.datasetKey) && a.catalogItemName.Equals(b.catalogItemName);
        }

        public static bool operator !=(CatalogItemRef a, CatalogItemRef b)
        {
            return !a.datasetKey.Equals(b.datasetKey) || !a.catalogItemName.Equals(b.catalogItemName);
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + datasetKey.GetHashCode();
            hash = (hash * 7) + catalogItemName.GetHashCode();

            return hash;
        }

        public override string ToString()
        {
            return string.Format("[CatalogItemRef catalogItemName={0}, datasetKey={1}]", catalogItemName, datasetKey);
        }
    }
}
