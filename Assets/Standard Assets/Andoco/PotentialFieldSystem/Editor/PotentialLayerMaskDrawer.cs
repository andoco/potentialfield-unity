using UnityEditor;
using UnityEngine;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Gui;

namespace Andoco.Unity.Framework.PotentialField
{
    [CustomPropertyDrawer(typeof(PotentialLayerMask))]
    public class PotentialLayerMaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var allData = PotentialFieldEditorHelper.LoadAllData();

            if (allData.Length == 0)
            {
                EditorGUI.LabelField(position, label, new GUIContent("No PotentialFieldData found"), EditorStyles.boldLabel);
                return;
            }

            position = EditorGUI.PrefixLabel(position, label);

            var splitPositions = position.SplitHorizontally(2);
            var datasetPos = splitPositions[0];
            var valuePos = splitPositions[1];

            var data = PotentialFieldEditorHelper.DatasetPopup(datasetPos, property, allData);

            PotentialFieldEditorHelper.LayerMaskPopup(valuePos, property, data);

            EditorGUI.EndProperty();
        }
    }
}
