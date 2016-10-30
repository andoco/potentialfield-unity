using System;
using System.Collections.Generic;
using Andoco.Unity.Framework.Gui;
using UnityEditor;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    [CustomEditor(typeof(PotentialFieldData))]
    public class PotentialFieldDataEditor : Editor
    {
        const string LayersFieldName = "layers";

        AutoReorderableList flagsList;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGuiHelper.AutoPropertyFields(
                serializedObject, 
                new [] { new KeyValuePair<string, Action>(LayersFieldName, () => flagsList.DoLayoutList()) });

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void OnEnable()
        {
            flagsList = new AutoReorderableList(serializedObject.FindProperty(LayersFieldName));
        }
    }
}
