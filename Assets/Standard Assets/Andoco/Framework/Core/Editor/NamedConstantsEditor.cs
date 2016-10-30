namespace Andoco.Unity.Framework.Core.Editor
{
    using Andoco.Unity.Framework.Gui;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(NamedConstants))]
    public class NamedConstantsEditor : Editor
    {
        private NamedConstants namedConstants;
        private ReorderableList list;

        private void OnEnable()
        {
            namedConstants = (NamedConstants)this.serializedObject.targetObject;

            list = new ReorderableList(
                serializedObject, 
                serializedObject.FindProperty("values"), 
                true, true, true, true);

            list.drawHeaderCallback = this.OnDrawHeader;
            list.drawElementCallback = this.OnDrawElement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Constant Values");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            var valueInstance = namedConstants.values[index];

            var valueField = string.Empty;

            switch (valueInstance.Type)
            {
                case NamedConstants.ConstType.Int:
                    valueField = "intValue";
                    break;
                case NamedConstants.ConstType.String:
                    valueField = "stringValue";
                    break;
                case NamedConstants.ConstType.Float:
                    valueField = "floatValue";
                    break;
            }

            EditorGuiHelper.ListElementPropertyFields(element, new [] { "Name", "name", "Group", "group", "", "type", "", valueField }, rect);
        }
    }
}