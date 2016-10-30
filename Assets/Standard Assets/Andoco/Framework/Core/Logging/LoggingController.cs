namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Andoco.Core.Diagnostics.Logging;

    public class LoggingController : MonoBehaviour {

		public bool debugLog;
        public bool addTraceListener;

        public SerializableLogLevelConfig defaultLogLevels;

        public List<RegexMatchLogLevelConfig> regexMatchLogLevels = new List<RegexMatchLogLevelConfig>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitLogging()
        {
            Debug.Log("Assigning UnityLoggerFactory to LogManager.LogFactory");
            LogManager.LogFactory = new UnityLoggerFactory();
        }

    	void Awake()
        {
            if (this.addTraceListener)
            {
                System.Diagnostics.Trace.Listeners.Add(new UnityTraceListener());
                UnityEngine.Debug.Log("UnityTraceListener has been installed. DEBUG symbol must be set to see output.");
            }

            Debug.Log("Assigning default logging levels to UnityLoggerFactory");

            var unityLogFactory = LogManager.LogFactory as UnityLoggerFactory;

            if (unityLogFactory == null)
            {
                Debug.LogWarningFormat(
                    "The current LogFactory is not of the expected type {0}. Actual type is {1}.", 
                    typeof(UnityLoggerFactory), 
                    LogManager.LogFactory.GetType());

                return;
            }

            UnityLoggerFactory.DefaultConfig = this.defaultLogLevels;
            unityLogFactory.RegexMatchLogLevels = this.regexMatchLogLevels;
    	}

        void Start()
        {
            // Initialize all loggers to have the default levels.
            // This won't have any affect on log writes prior to this script Start() event.
            var unityLogFactory = LogManager.LogFactory as UnityLoggerFactory;
            if (unityLogFactory != null)
            {
                unityLogFactory.EnsureLoggersInitialized();
            }
        }
    }
}