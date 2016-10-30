namespace Andoco.Unity.Framework.Core.Diagnostics.Editor
{
    using UnityEngine;
    using UnityEditor;
    using Andoco.Unity.Framework.Core.DebugConsole;
    using Andoco.Unity.Framework.Gui;
    using System.Collections.Generic;
    using System;

    [CustomEditor(typeof(DebugConsole))]
    public class DebugConsoleEditor : Editor
    {
        private const string AliasesFieldName = "aliases";

        private AutoReorderableList list;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGuiHelper.AutoPropertyFields(
                serializedObject,
                new[] { new KeyValuePair<string, Action>(AliasesFieldName, () => list.DoLayoutList()) });

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnEnable()
        {
            list = new AutoReorderableList(serializedObject.FindProperty(AliasesFieldName));
        }
    }
}