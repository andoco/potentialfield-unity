namespace Andoco.Unity.Framework.Inventory.Editor
{
    using Andoco.Unity.Framework.Core;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(CatalogItemRef))]
    public class CatalogItemRefDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var allData = InventoryEditorHelper.LoadAllData();

            if (allData.Length == 0)
            {
                EditorGUI.LabelField(position, label, new GUIContent("No item catalog data found"), EditorStyles.boldLabel);
                return;
            }

            position = EditorGUI.PrefixLabel(position, label);

            var splitPositions = position.SplitHorizontally(2);
            var datasetPos = splitPositions[0];
            var valuePos = splitPositions[1];

            var data = InventoryEditorHelper.DatasetPopup(datasetPos, property, allData);

            InventoryEditorHelper.ItemPopup(valuePos, property, data);

            EditorGUI.EndProperty();
        }
    }
}