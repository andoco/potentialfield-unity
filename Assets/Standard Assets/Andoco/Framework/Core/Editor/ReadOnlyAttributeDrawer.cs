namespace Andoco.Unity.Framework.Core.Editor
{
    using Andoco.Unity.Framework.Core;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributeDrawer : PropertyDrawer
    {
//        public override float GetPropertyHeight(SerializedProperty property,
//            GUIContent label)
//        {
//            return EditorGUI.GetPropertyHeight(property, label, true);
//        }

        public override void OnGUI(Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            var prop = (ReadOnlyAttribute)this.attribute;

            var editable = false;

            if (prop.RuntimeOnly && !Application.isPlaying)
                editable = true;

            if (!editable)
                GUI.enabled = false;
            
            EditorGUI.PropertyField(position, property, label, true);

            if (!editable)
                GUI.enabled = true;
        }
    }
}