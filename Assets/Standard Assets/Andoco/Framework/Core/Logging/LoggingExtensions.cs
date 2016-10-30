namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using UnityEngine;
    using Andoco.Core.Diagnostics.Logging;

    public static class LoggingExtensions
    {
        public static IStructuredLog GetStructuredLog(this UnityEngine.Object obj)
        {
            return LogManager.GetStructuredLog(obj.name, obj.GetType());
        }
    }
}