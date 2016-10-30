namespace Andoco.Unity.Framework.Inventory.Editor
{
    using System;
    using System.Collections.Generic;
    using Andoco.Unity.Framework.Gui;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(InventorySystem))]
    public class InventorySystemEditor : Editor
    {
        const string itemHandlersFieldName = "itemHandlers";
        AutoReorderableList flagsList;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGuiHelper.AutoPropertyFields(
                serializedObject,
                new[] { new KeyValuePair<string, Action>(itemHandlersFieldName, () => flagsList.DoLayoutList()) });

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void OnEnable()
        {
            flagsList = new AutoReorderableList(
                serializedObject.FindProperty(itemHandlersFieldName), 
                AutoReorderableList.ElementLayoutKind.Vertical);
        }
    }
}