namespace Andoco.Unity.Framework.Misc.Editor
{
    using UnityEngine;
    using UnityEditor;
    using Andoco.Unity.Framework.Gui;
    using System.Collections.Generic;
    using System;

    [CustomEditor(typeof(Damage))]
    public class DamageEditor : Editor
    {
        private const string ProfilesFieldName = "profiles";

        private AutoReorderableList list;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGuiHelper.AutoPropertyFields(
                serializedObject,
                new[] { new KeyValuePair<string, Action>(ProfilesFieldName, () => list.DoLayoutList()) });

            if (GUI.changed)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void OnEnable()
        {
            list = new AutoReorderableList(
                serializedObject.FindProperty(ProfilesFieldName),
                AutoReorderableList.ElementLayoutKind.Vertical);
        }
    }
}
