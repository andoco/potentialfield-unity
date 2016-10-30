namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using Andoco.Core.Diagnostics.Logging;

    public class UnityStructuredLogEventWriter : IStructuredLogEventWriter
    {
        public void Write(LogLevel level, string message, KeyValuePair<string, object>[] fields)
        {
            var s = new StringBuilder();

            s.AppendFormat("{0} [{1}] {2}:", Time.time, level, message);

            foreach (var kv in fields)
            {
//                s.AppendFormat(" <b>{0}</b>={1} ", kv.Key, kv.Value);
                s.AppendFormat("\n{0}={1} ", kv.Key, kv.Value);
            }

            var fullMsg = s.ToString();

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