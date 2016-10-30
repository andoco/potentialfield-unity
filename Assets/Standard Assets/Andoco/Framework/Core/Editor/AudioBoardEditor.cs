namespace Andoco.Unity.Framework.Core.Editor
{
    using System;
    using Andoco.Unity.Framework.Gui;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(AudioBoard))]
    public class AudioBoardEditor : Editor
    {
        private ReorderableList list;

        private void OnEnable() {
            list = new ReorderableList(
                serializedObject, 
                serializedObject.FindProperty("audioClips"), 
                true, true, true, true);

            list.drawHeaderCallback = this.OnDrawHeader;
            list.drawElementCallback = this.OnDrawElement;
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Audio Clips");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            var fields = new [] { "Key", "key", "Clip", "clip", "Global", "global" };

            EditorGuiHelper.ListElementPropertyFields(element, fields, rect);
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
