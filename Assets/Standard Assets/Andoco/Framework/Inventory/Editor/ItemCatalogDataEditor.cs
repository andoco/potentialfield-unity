namespace Andoco.Unity.Framework.Inventory.Editor
{
    using System;
    using System.Collections.Generic;
    using Andoco.Unity.Framework.Gui;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ItemCatalogData))]
    public class ItemCatalogDataEditor : Editor
    {
        const string itemsFieldName = "items";
        AutoReorderableList flagsList;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGuiHelper.AutoPropertyFields(
                serializedObject,
                new[] { new KeyValuePair<string, Action>(itemsFieldName, () => flagsList.DoLayoutList()) });

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void OnEnable()
        {
            flagsList = new AutoReorderableList(serializedObject.FindProperty(itemsFieldName), AutoReorderableList.ElementLayoutKind.Vertical);
        }
    }
}