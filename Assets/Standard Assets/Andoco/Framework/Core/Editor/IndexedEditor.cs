namespace Andoco.Unity.Framework.Core.Editor
{
    using System;
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;

    [CustomEditor(typeof(Indexed))]
    public class IndexedEditor : Editor
    {
        private ReorderableList list;

        private void OnEnable() {
            list = new ReorderableList(
                serializedObject, 
                serializedObject.FindProperty("behavioursToIndex"), 
                true, true, true, true);

            list.drawHeaderCallback = this.OnDrawHeader;
            list.drawElementCallback = this.OnDrawElement;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Indexed Components");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            rect.y += 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element,
                GUIContent.none);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            list.DoLayoutList();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("indexByType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("indexByInterfaces"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bindZenject"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}
