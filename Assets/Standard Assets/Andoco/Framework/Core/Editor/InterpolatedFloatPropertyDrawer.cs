namespace Andoco.Unity.Framework.Core.Editor
{
    using Andoco.Unity.Framework.Core;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(InterpolatedFloat))]
    public class InterpolatedFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty (position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel (position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var startRect = new Rect(position.x, position.y, 40, position.height);
            var endRect = new Rect(position.x+45, position.y, 40, position.height);
            var easeTypeRect = new Rect(position.x+90, position.y, position.width-90, position.height);

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(startRect, property.FindPropertyRelative ("start"), GUIContent.none);
            EditorGUI.LabelField(startRect, new GUIContent("", "The start value of the interpolated range"));
            EditorGUI.PropertyField(endRect, property.FindPropertyRelative ("end"), GUIContent.none);
            EditorGUI.LabelField(endRect, new GUIContent("", "The end value of the interpolated range"));
            EditorGUI.PropertyField(easeTypeRect, property.FindPropertyRelative ("easeType"), GUIContent.none);
            EditorGUI.LabelField(easeTypeRect, new GUIContent("", "The interpolation function to use"));

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty ();
        }
    }
}