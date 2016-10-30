namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Unity.Framework.Core.Logging;
    using UnityEngine;
    using Zenject;

    public class Logging : MonoBehaviour, ILogLevelConfig
    {
        public bool trace;
        public bool debug;
        public bool info;
        public bool warning;
        public bool error;
        public bool fatal;

        public bool Check(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    return this.trace;
                case LogLevel.Debug:
                    return this.debug;
                case LogLevel.Info:
                    return this.info;
                case LogLevel.Warn:
                    return this.warning;
                case LogLevel.Error:
                    return this.error;
                case LogLevel.Fatal:
                    return this.fatal;
                default:
                    throw new InvalidOperationException(string.Format("Cannot check value of unknown log level {0}", level));
            }
        }
    }
}
