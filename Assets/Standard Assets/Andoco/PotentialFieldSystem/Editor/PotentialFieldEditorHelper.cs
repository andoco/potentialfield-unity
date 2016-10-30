using System;
using System.Linq;
using Andoco.Unity.Framework.Core;
using Andoco.Unity.Framework.Gui;
using UnityEditor;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public static class PotentialFieldEditorHelper
    {
        public static PotentialFieldData[] LoadAllData()
        {
            return Resources.LoadAll<PotentialFieldData>("");
        }

        public static PotentialFieldData DatasetPopup(Rect position, SerializedProperty property, PotentialFieldData[] allData)
        {
            var datasetKeyProp = property.FindPropertyRelative("datasetKey");
            var datasetIndex = Array.FindIndex(allData, x => x.datasetKey == datasetKeyProp.stringValue);

            if (datasetIndex == -1)
                datasetIndex = 0;

            datasetIndex = EditorGUI.Popup(
                position, 
                datasetIndex, 
                allData.Select(x => x.datasetKey).ToArray());

            var plotData = allData[datasetIndex];

            datasetKeyProp.stringValue = plotData.datasetKey;

            return plotData;
        }

        public static int LayerPopup(Rect position, SerializedProperty property, PotentialFieldData data)
        {
            var valueProp = property.FindPropertyRelative("value");

            var values = data.layers.Select(x => x.value).ToArray();
            var labels = data.layers.Select(x => new GUIContent(x.name)).ToArray();

            valueProp.intValue = EditorGUI.IntPopup(position, valueProp.intValue, labels, values);

            return valueProp.intValue;
        }

        public static int LayerMaskPopup(Rect position, SerializedProperty property, PotentialFieldData plotData)
        {
            var valueProp = property.FindPropertyRelative("value");

            var labels = plotData.layers.Select(x => x.name).ToArray();

            valueProp.intValue = EditorGUI.MaskField(position, valueProp.intValue, labels);

            return valueProp.intValue;
        }
    }
}
