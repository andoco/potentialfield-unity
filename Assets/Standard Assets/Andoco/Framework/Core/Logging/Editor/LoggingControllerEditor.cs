namespace Andoco.Unity.Framework.Core.Logging
{
    using System.Collections;
    using UnityEditor;
    using UnityEngine;
    using Andoco.Core;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Gui;

    [CustomEditor(typeof(LoggingController))]
    public class LoggingControllerEditor : Editor
    {
        private GUIStyle style1;
        private GUIStyle style2;
        private GuiHelper guiHelper;

        void OnEnable()
        {
            this.guiHelper = new GuiHelper();
            this.style1 = new GUIStyle();
            this.style2 = new GUIStyle();
            this.style2.normal.background = GuiHelper.MakeTex(600, 1, new Color(1.0f, 1.0f, 1.0f, 0.5f));            
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("debugLog"));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("addTraceListener"));
			this.guiHelper.InspectorSeparator();

            var controller = (LoggingController)this.target;
            var manager = LogManager.LogFactory as UnityLoggerFactory;

			if (controller.debugLog)
				Debug.LogFormat(this, "Current LogManager.LogFactory = {0}", manager);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Default Log Levels");
            this.DrawLogLevelConfig(controller.defaultLogLevels);

			this.DrawLoggerConfigs(controller, manager);
			this.DrawActiveLoggers(manager);

            if(GUI.changed)
            {
				EditorUtility.SetDirty(controller);
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        private void DrawLogLevelConfig(LogLevelConfig cfg)
        {
            GUILayout.BeginHorizontal();
            cfg.Trace =  GUILayout.Toggle(cfg.Trace, "Trace");
            cfg.Debug =  GUILayout.Toggle(cfg.Debug, "Debug");
            cfg.Info =  GUILayout.Toggle(cfg.Info, "Info");
            cfg.Warning =  GUILayout.Toggle(cfg.Warning, "Warning");
            cfg.Error =  GUILayout.Toggle(cfg.Error, "Error");
            cfg.Fatal =  GUILayout.Toggle(cfg.Fatal, "Fatal");
            GUILayout.EndHorizontal();
        }

		private void DrawLoggerConfigs(LoggingController controller, UnityLoggerFactory factory)
		{
			this.guiHelper.InspectorSeparator();

			EditorGUILayout.LabelField("Regex Match Log Levels");

			RegexMatchLogLevelConfig toDelete = null;

			foreach (var cfg in controller.regexMatchLogLevels)
			{
				EditorGUILayout.BeginHorizontal();

				if (GUILayout.Button("X"))
				{
					toDelete = cfg;
				}

				EditorGUILayout.BeginVertical();
				cfg.Regex = EditorGUILayout.TextField("Pattern", cfg.Regex);
				this.DrawLogLevelConfig(cfg);
				EditorGUILayout.EndVertical();

				EditorGUILayout.EndHorizontal();
			}

			if (toDelete != null)
			{
				controller.regexMatchLogLevels.Remove(toDelete);
			}

			if (GUILayout.Button("Add"))
			{
				var cfg = new RegexMatchLogLevelConfig();
				cfg.CopyFrom(UnityLoggerFactory.DefaultConfig);
				controller.regexMatchLogLevels.Add(cfg);
			}
		}

		private void DrawActiveLoggers(UnityLoggerFactory factory)
		{
			this.guiHelper.InspectorSeparator();

			EditorGUILayout.LabelField("Active Loggers");

			if (EditorApplication.isPlaying || EditorApplication.isPaused)
			{
				var currentStyle = style1;

				foreach (var item in factory.LogConfigs)
				{
					var cfg = item.Value;

					GUILayout.BeginVertical(currentStyle);

					//                    EditorGUILayout.LabelField("{0} - {1}".FormatWith(item.Key.Source.SourceType.FullName, item.Key.Source.Name));
					EditorGUILayout.LabelField(item.Key.Source.FullName);

					this.DrawLogLevelConfig(cfg);

					GUILayout.EndVertical();

					currentStyle = currentStyle == style1 ? style2 : style1;
				}
			}
			else
			{
				GUILayout.Label("Loggers will be listed here when playing");
			}
		}
    }
}