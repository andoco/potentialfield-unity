namespace Andoco.Unity.Framework.Core.Editor
{
    using System;
    using System.Collections.Generic;
    using Andoco.Unity.Framework.Gui;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Bootstrap))]
    public class BootstrapEditor : Editor
    {
        private const string PrefabsFieldName = "prefabs";

        private AutoReorderableList list;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGuiHelper.AutoPropertyFields(
                serializedObject,
                new[] { new KeyValuePair<string, Action>(PrefabsFieldName, () => list.DoLayoutList()) });

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnEnable()
        {
            list = new AutoReorderableList(serializedObject.FindProperty(PrefabsFieldName));
        }
    }
}

