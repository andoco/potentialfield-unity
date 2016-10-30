using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditorInternal;

namespace Andoco.Unity.Framework.WorldBuilding.Components
{
    [CustomEditor(typeof(WorldBuilder))]
    public class WorldBuilderEditor : Editor
    {
        private ReorderableList list;

        private void OnEnable()
        {
            list = new ReorderableList(
                serializedObject, 
                serializedObject.FindProperty("layers"), 
                true, true, true, true);

            list.drawHeaderCallback = this.OnDrawHeader;
            list.drawElementCallback = this.OnDrawElement;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Layers");
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

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            list.DoLayoutList();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("worldBuilt"));
            serializedObject.ApplyModifiedProperties();
        }
    }
}