namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using UnityEngine;
    using Andoco.Core.Diagnostics.Logging;

    public class UnityLogger : Andoco.Core.Diagnostics.Logging.StandardLogger
    {
        public UnityLogger(ILogSource source, ILogLevelConfig config)
            : base(source, new LogLevelStatus(config))
        {
        }

        protected override void InternalWrite(LogLevel level, object message, System.Exception exception, object[] args)
        {
            object formattedMsg;
            if (args.Length > 0)
                formattedMsg = string.Format((string)message, args);
            else
                formattedMsg = message;

            var fullMsg = string.Format("{0} {1} {2} {3}", this.Source.Name, Time.time, level.ToString().ToUpper(), formattedMsg);
            
            switch (level)
            {
            case LogLevel.Debug:
            case LogLevel.Trace:
            case LogLevel.Info:
                UnityEngine.Debug.Log(fullMsg);
                break;
                
            case LogLevel.Warn:
                UnityEngine.Debug.LogWarning(fullMsg);
                break;
                
            case LogLevel.Error:
            case LogLevel.Fatal:
                UnityEngine.Debug.LogError(fullMsg);
                break;
            }
        }
    }
}