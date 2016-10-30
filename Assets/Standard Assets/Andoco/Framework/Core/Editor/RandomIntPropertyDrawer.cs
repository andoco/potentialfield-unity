namespace Andoco.Unity.Framework.Core.Editor
{
    using Andoco.Unity.Framework.Core;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(RandomInt))]
    public class RandomIntPropertyDrawer : PropertyDrawer
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
            var minRect = new Rect(position.x, position.y, 40, position.height);
            var maxRect = new Rect(position.x+45, position.y, 40, position.height);

            // Draw fields
            EditorGUI.PropertyField(minRect, property.FindPropertyRelative ("min"), GUIContent.none);
            EditorGUI.LabelField(minRect, new GUIContent("", "The inclusive minimum value of the random number range"));
            EditorGUI.PropertyField(maxRect, property.FindPropertyRelative("max"), GUIContent.none);
            EditorGUI.LabelField(maxRect, new GUIContent("", "The exclusive maximum value of the random number range"));

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty ();
        }
    }
}