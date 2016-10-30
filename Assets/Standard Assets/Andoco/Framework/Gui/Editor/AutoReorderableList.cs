using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Andoco.Unity.Framework.Core;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Gui
{
    public class AutoReorderableList
    {
        ReorderableList list;
        GUIContent header;
        string[] labelsAndProps;

        public AutoReorderableList(SerializedProperty property, ElementLayoutKind layout = ElementLayoutKind.Horizontal)
            : this(property, new GUIContent(property.displayName), layout)
        {
        }

        public AutoReorderableList(SerializedProperty property, GUIContent header, ElementLayoutKind layout = ElementLayoutKind.Horizontal)
        {
            Assert.IsTrue(property.isArray);

            this.header = header;
            this.Layout = layout;

            labelsAndProps = GetElementLabelsAndNames(property);

            var iter = property.Copy();

            // Read the size of the array first
            iter.NextVisible(true);

            // Read the first element in array
            iter.NextVisible(true);

            // Get the property label and name from the first element.
            //labelsAndProps = GetElementLabelsAndNames(iter);

            list = new ReorderableList(
                property.serializedObject, 
                property, 
                true, true, true, true);

            list.drawHeaderCallback = this.OnDrawHeader;
            list.drawElementCallback = this.OnDrawElement;
            //list.elementHeightCallback = this.OnElementHeight;
            list.elementHeight = GetElementHeight();
        }

        public AutoReorderableList(SerializedProperty property, GUIContent header, string[] labelsAndProps, ElementLayoutKind layout = ElementLayoutKind.Horizontal)
        {
            list = new ReorderableList(
                property.serializedObject, 
                property, 
                true, true, true, true);

            this.header = header;
            this.labelsAndProps = labelsAndProps;
            this.Layout = layout;

            list.drawHeaderCallback = this.OnDrawHeader;
            list.drawElementCallback = this.OnDrawElement;
            //list.elementHeightCallback = this.OnElementHeight;
            list.elementHeight = GetElementHeight();
        }

        public ElementLayoutKind Layout { get; set; }

        public void DoLayoutList()
        {
            list.DoLayoutList();
        }

        static string[] GetElementLabelsAndNames(SerializedProperty prop)
        {
            //Debug.LogFormat("Getting labels and names for {0}", prop.propertyPath);

            var obj = prop.serializedObject.targetObject;
            var arrayField = obj.GetType().GetField(prop.name);
            var elemType = arrayField.FieldType.GetElementType();
            var elemFields = elemType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic); // TODO: Handle private fields with SerializeField
            var results = new List<string>();

            foreach (var f in elemFields)
            {
                // Skip non serialized fields.
                if (f.GetCustomAttributes(false).Any(a => a is System.NonSerializedAttribute))
                    continue;

                // Only add public, or fields with SerializeField.
                if (!(f.IsPublic || f.GetCustomAttributes(false).Any(a => a is SerializeField)))
                    continue;
                
                results.Add(ObjectNames.NicifyVariableName(f.Name));
                results.Add(f.Name);
            }

            return results.ToArray();
        }

        void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, header);
        }

        float OnElementHeight(int index)
        {
            return GetElementHeight();
        }

        void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);

            //Debug.LogFormat("drawing element {0}", element.name);

            if (Layout == ElementLayoutKind.Horizontal)
            {
                EditorGuiHelper.ListElementPropertyFields(element, labelsAndProps, rect);
            }
            else
            {
                var rects = rect.SplitVertically(labelsAndProps.Length / 2, EditorGUIUtility.standardVerticalSpacing);

                for (int i = 0; i < labelsAndProps.Length; i+=2)
                {
                    //Debug.LogFormat("drawing field {0}", labelsAndProps[i]);

                    var r = rects[i / 2];

                    EditorGUI.PropertyField(r, element.FindPropertyRelative(labelsAndProps[i + 1]));
                }
            }
        }

        float GetElementHeight()
        {
            switch (Layout)
            {
                case ElementLayoutKind.Horizontal:
                    return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                case ElementLayoutKind.Vertical:
                    return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * (labelsAndProps.Length / 2);
                default:
                    return EditorGUIUtility.singleLineHeight;
            }
        }

        public enum ElementLayoutKind
        {
            Horizontal,
            Vertical
        }
    }
}
