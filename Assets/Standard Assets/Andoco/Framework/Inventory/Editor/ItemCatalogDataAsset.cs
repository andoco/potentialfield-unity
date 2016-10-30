namespace Andoco.Unity.Framework.Inventory.Editor
{
    using Andoco.Unity.Framework.Core.Editor;
    using UnityEditor;

    public class ItemCatalogDataAsset
    {
        [MenuItem("Assets/Create/ItemCatalogData")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<ItemCatalogData>();
        }
    }
}