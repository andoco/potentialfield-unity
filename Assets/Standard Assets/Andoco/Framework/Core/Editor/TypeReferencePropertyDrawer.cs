namespace Andoco.Unity.Framework.Core.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Andoco.Core;
    using Andoco.Core.Reflection;
    using Andoco.Unity.Framework.Core;
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(TypeReferenceAttribute))]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {
        private class TypeRefInfo
        {
            public Type[] types;
            public string[] options;
        }

        private static IDictionary<Type, TypeRefInfo> typeLookup = new Dictionary<Type, TypeRefInfo>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attribute = this.attribute as TypeReferenceAttribute;
            var baseType = attribute.ReferencedBaseType;

            TypeRefInfo typeInfo;

            if (!typeLookup.TryGetValue(baseType, out typeInfo))
            {
                typeInfo = new TypeRefInfo();

                typeInfo.types = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.FindAssignableTypes(baseType)).Filter(TypeFilterFlags.Concrete).ToArray();

                var displayOptions = typeInfo.types.Select(x => x.Name).ToList();

                if (attribute.NullOption)
                    displayOptions.Insert(0, "Select type");
                
                typeInfo.options = displayOptions.ToArray();

                typeLookup.Add(baseType, typeInfo);
            }

            // Get the index of the currently chosen type.
            var typeIndex = Array.IndexOf(typeInfo.types, Type.GetType(property.stringValue));

            // Work out the selected option index to display.
            var optionIndex = typeIndex == -1
                ? 0
                : attribute.NullOption
                    ? typeIndex + 1
                    : typeIndex;

//            Debug.LogFormat("BEFORE: typeIndex={0}, optionIndex={1}", typeIndex, optionIndex);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            
            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            EditorGUI.BeginChangeCheck();

            optionIndex = EditorGUI.Popup(position, optionIndex, typeInfo.options);

            // Work out the index of the new chosen type.
            typeIndex = attribute.NullOption
                ? optionIndex - 1
                : optionIndex;

//            Debug.LogFormat("AFTER: typeIndex={0}, optionIndex={1}", typeIndex, optionIndex);

            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = typeIndex >= 0
                    ? typeInfo.types[typeIndex].AssemblyQualifiedName
                    : null;

//                Debug.LogFormat("VALUE: {0}", property.stringValue);
            }
            
            EditorGUI.EndProperty();
        }
    }
}