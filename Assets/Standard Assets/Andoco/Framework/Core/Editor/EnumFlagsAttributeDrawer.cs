namespace Andoco.Unity.Framework.Core.Editor
{
    using Andoco.Unity.Framework.Core;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.MaskField( position, label, property.intValue, property.enumNames );
        }
    }
}