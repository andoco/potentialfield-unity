namespace Andoco.Unity.Framework.Inventory.Editor
{
    using System;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public static class InventoryEditorHelper
    {
        public static ItemCatalogData[] LoadAllData()
        {
            return Resources.LoadAll<ItemCatalogData>("");
        }

        public static ItemCatalogData DatasetPopup(Rect position, SerializedProperty property, ItemCatalogData[] allData)
        {
            var datasetKeyProp = property.FindPropertyRelative("datasetKey");
            var datasetIndex = Array.FindIndex(allData, x => x.datasetKey == datasetKeyProp.stringValue);

            if (datasetIndex == -1)
                datasetIndex = 0;

            datasetIndex = EditorGUI.Popup(
                position, 
                datasetIndex, 
                allData.Select(x => x.datasetKey).ToArray());

            var plotData = allData[datasetIndex];

            datasetKeyProp.stringValue = plotData.datasetKey;

            return plotData;
        }

        public static int ItemPopup(Rect position, SerializedProperty property, ItemCatalogData data)
        {
            var nameProp = property.FindPropertyRelative("catalogItemName");

            var names = data.items.Select(x => x.name).ToArray();
            var labels = data.items.Select(x => new GUIContent(x.displayName)).ToArray();

            var selectedIndex = Array.IndexOf(names, nameProp.stringValue);
            if (selectedIndex == -1)
                selectedIndex = 0;
            selectedIndex = EditorGUI.Popup(position, selectedIndex, labels);
            nameProp.stringValue = names[selectedIndex];

            return selectedIndex;
        }
    }
}