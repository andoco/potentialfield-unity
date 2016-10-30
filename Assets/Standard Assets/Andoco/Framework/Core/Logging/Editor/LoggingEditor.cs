namespace Andoco.Unity.Framework.Core.Logging
{
    using Andoco.Core.Diagnostics.Logging;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(Logging))]
    public class LoggingEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            var logging = (Logging)this.target;

            Undo.RecordObject(logging, "Logging log levels");

            this.ShowLogLevels(logging);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("All"))
            {
                this.SetAll(logging, true);
            }

            if (GUILayout.Button("None"))
            {
                this.SetAll(logging, false);
            }

            EditorGUILayout.EndHorizontal();

            if(GUI.changed)
            {
				EditorUtility.SetDirty(logging);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        private void ShowLogLevels(Logging logging)
        {
            EditorGUILayout.BeginHorizontal();
            logging.trace = GUILayout.Toggle(logging.trace, "Trace", "Button");
            logging.debug = GUILayout.Toggle(logging.debug, "Debug", "Button");
            logging.info = GUILayout.Toggle(logging.info, "Info", "Button");
            logging.warning = GUILayout.Toggle(logging.warning, "Warning", "Button");
            logging.error = GUILayout.Toggle(logging.error, "Error", "Button");
            logging.fatal = GUILayout.Toggle(logging.fatal, "Fatal", "Button");
            EditorGUILayout.EndHorizontal();
        }

        private void SetAll(Logging logging, bool value)
        {
            logging.trace = value;
            logging.debug = value;
            logging.info = value;
            logging.warning = value;
            logging.error = value;
            logging.fatal = value;
        }
    }
}