namespace Andoco.Unity.Framework.Core.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System.Collections;

    [CustomEditor(typeof(ObjectPoolController))]
    public class ObjectPoolEditor : Editor
    {
        private ReorderableList list;

        private void OnEnable() {
            list = new ReorderableList(
                serializedObject, 
                serializedObject.FindProperty("autoPoolPrefabs"), 
                true, true, true, true);

            list.drawHeaderCallback = this.OnDrawHeader;
            list.drawElementCallback = this.OnDrawElement;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Auto Pool");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            rect.y += 2;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, 200f, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("prefab"),
                GUIContent.none);

            EditorGUI.PropertyField(
                new Rect(rect.x + 200f, rect.y, rect.width - 200f, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("quantity"),
                GUIContent.none);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("poolEnabled"));
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}