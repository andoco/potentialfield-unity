using Andoco.Unity.Framework.Gui;
using UnityEditor;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    [CustomEditor(typeof(PotentialFieldSource))]
    public class PotentialFieldSourceEditor : Editor
    {
        GuiHelper guiHelper;

        void OnEnable()
        {
            guiHelper = new GuiHelper();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var target = (PotentialFieldSource)this.serializedObject.targetObject;

            guiHelper.InspectorSeparator();

            EditorGUILayout.LabelField("Node Sources", EditorStyles.largeLabel);

            foreach (var nodeSource in target.GetNodeSources())
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("SourceKey", nodeSource.SourceKey);
                GUI.enabled = false;
                EditorGUILayout.IntField("Layers", nodeSource.Layers);
                EditorGUILayout.FloatField("Potential", nodeSource.Potential);

                if (nodeSource.Node == null)
                    EditorGUILayout.LabelField("Node", "-");
                else
                    EditorGUILayout.Vector3Field("Node", nodeSource.Node.Position);
                
                EditorGUILayout.Toggle("Enabled", nodeSource.Enabled);
                GUI.enabled = true;
            }
        }
    }
}
