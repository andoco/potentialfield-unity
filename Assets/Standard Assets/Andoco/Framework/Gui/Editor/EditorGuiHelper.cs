using System;
using System.Collections.Generic;
using System.Linq;
using Andoco.Unity.Framework.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.Gui
{
    public static class EditorGuiHelper
    {
        public static string[] ExcludedAutoPropertyFields = new [] { "m_Script" };

        public static void AutoPropertyFields(SerializedObject serializedObject, params KeyValuePair<string, Action>[] overrides)
        {
            var prop = serializedObject.GetIterator();

            SerializedProperty skipTo = null;

            while (prop.NextVisible(true))
            {
                // We need to skip properties first to make sure we don't go past the final property.
                if (skipTo != null)
                {
                    if (!SerializedProperty.EqualContents(skipTo, prop))
                        continue;

                    skipTo = null;
                }
                
                if (Array.Exists(ExcludedAutoPropertyFields, x => x == prop.name))
                    continue;
                
                //Debug.LogFormat("name={0}, displayName={1}, editable={2}, hasChildren={3}, hasVisibleChildren={4}", prop.name, prop.displayName, prop.editable, prop.hasChildren, prop.hasVisibleChildren);

                var overrideIndex = Array.FindIndex(overrides, x => x.Key == prop.name);

                if (overrideIndex >= 0)
                {
                    overrides[overrideIndex].Value();
                }
                else
                {
                    EditorGUILayout.PropertyField(prop, true);
                }

                // We've already drawn the child properties in the call above, so we need to skip them.
                if (prop.hasVisibleChildren)
                {
                    skipTo = prop.GetEndProperty(false);
                }
            }
        }

        public static void PropertyFields(SerializedObject serializedObject, params string[] fields)
        {
            foreach (var field in fields)
            {
                var prop = serializedObject.FindProperty(field);
                EditorGUILayout.PropertyField(prop, true);
            }
        }

        public static void ListElementPropertyFields(SerializedProperty element, string[] props, Rect rect)
        {
            Assert.IsTrue(props.Length % 2 == 0);

            //            Debug.LogFormat("rect = {0}", rect);

            var numItems = props.Length;
            var rects = new Rect[numItems];

            for (int i = 0; i < numItems; i++)
            {
                var r = new Rect();
                r.y = rect.y;

                if (i % 2 == 0)
                    r.width = GUI.skin.label.CalcSize(new GUIContent(props[i])).x;
                else
                    r.width = 0f; // we'll work out field width after we know all label widths.

                r.height = EditorGUIUtility.singleLineHeight;

                rects[i] = r;
            }

            var x = rect.x;
            var totalWidth = rects.Sum(r => r.width);

            //            Debug.LogFormat("Total width after label calculation = {0}", totalWidth);

            for (int i = 0; i < numItems; i++)
            {
                rects[i].x = x;

                if (i % 2 == 1)
                    rects[i].width = (rect.width - totalWidth) / (numItems / 2);

                x += rects[i].width;
            }

            for (int i = 0; i < numItems; i++)
            {
                if (i % 2 == 0)
                {
                    EditorGUI.LabelField(rects[i], props[i]);
                }
                else
                {
                    EditorGUI.PropertyField(
                        rects[i],
                        element.FindPropertyRelative(props[i]),
                        GUIContent.none);
                }
            }
        }

        public static void NamedConstantGroupIntPopup(SerializedObject serializedObject, string label, string propertyName, NamedConstants constants, string groupName)
        {
            NamedConstantGroupIntPopup(serializedObject, label, propertyName, constants, groupName);
        }

        public static void NamedConstantGroupIntPopup(SerializedObject serializedObject, string label, string propertyName, NamedConstants constants, string groupName, Rect? position)
        {
            if (constants == null)
            {
                EditorGUILayout.LabelField(label, "<missing constants>");
                return;
            }
            
            var property = serializedObject.FindProperty(propertyName);
            var values = constants.GetGroup(groupName);
            var labels = values.Select(x => x.Name).ToArray();
            var layers = values.Select(x => x.IntValue).ToArray();

            if (position.HasValue)
                property.intValue = EditorGUI.IntPopup(position.Value, label, property.intValue, labels, layers);
            else
                property.intValue = EditorGUILayout.IntPopup(label, property.intValue, labels, layers);
        }

        public static void NamedConstantGroupMaskPopup(SerializedObject serializedObject, string label, string propertyName, NamedConstants constants, string groupName)
        {
            NamedConstantGroupMaskPopup(serializedObject, label, propertyName, constants, null);
        }

        public static void NamedConstantGroupMaskPopup(SerializedObject serializedObject, string label, string propertyName, NamedConstants constants, string groupName, Rect? position)
        {
            if (constants == null)
            {
                EditorGUILayout.LabelField(label, "<missing constants>");
                return;
            }

            var property = serializedObject.FindProperty(propertyName);
            var values = constants.GetGroup(groupName);
            var labels = values.Select(x => x.Name).ToArray();

            if (position == null)
            {
                property.intValue = EditorGUILayout.MaskField(label, property.intValue, labels);
            }
            else
            {
                property.intValue = EditorGUI.MaskField(position.Value, label, property.intValue, labels);
            }
        }
    }
}
