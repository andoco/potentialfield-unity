using System;
using System.Collections.Generic;
using Andoco.Unity.Framework.Gui;
using UnityEditor;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    [CustomEditor(typeof(TrackingPotentialFieldSource))]
    public class TrackingPotentialFieldSourceEditor : Editor
    {
        const string SourceConfigsFieldName = "sourceConfigs";

        AutoReorderableList flagsList;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGuiHelper.AutoPropertyFields(
                serializedObject, 
                new [] { new KeyValuePair<string, Action>(SourceConfigsFieldName, () => flagsList.DoLayoutList()) });

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        void OnEnable()
        {
            flagsList = new AutoReorderableList(
                serializedObject.FindProperty(SourceConfigsFieldName), 
                AutoReorderableList.ElementLayoutKind.Vertical);
        }
    }
}
