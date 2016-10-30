namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Andoco.Core.Diagnostics.Logging;

    public class UnityLoggerFactory : LoggerFactory
    {
        private UnityStructuredLogEventWriter unityStructuredLogEventWriter;

        public static LogLevelConfig DefaultConfig { get; set; }

        public UnityLoggerFactory()
        {
            DefaultConfig = new LogLevelConfig();
            this.LogConfigs = new Dictionary<IConfigurableLog, LogLevelConfig>();
            this.unityStructuredLogEventWriter = new UnityStructuredLogEventWriter();
        }

        public IDictionary<IConfigurableLog, LogLevelConfig> LogConfigs { get; private set; }

        public IList<RegexMatchLogLevelConfig> RegexMatchLogLevels { get; set; }

        public void EnsureLoggersInitialized()
        {
            foreach (var item in this.LogConfigs)
            {
                var sourceConfig = this.PickConfig(item.Key.Source);
                item.Value.CopyFrom(sourceConfig);
            }
        }

        protected override IStandardLog CreateLoggerInstance(ILogSource source)
        {
            var config = new LogLevelConfig();
            config.CopyFrom(this.PickConfig(source));

            var logger = new UnityLogger(source, config);

            this.LogConfigs.Add(logger, config);

            return logger;
        }

        protected override IStructuredLog CreateStructuredLogInstance(ILogSource source)
        {
            var config = new LogLevelConfig();
            config.CopyFrom(this.PickConfig(source));

            var logger = new StructuredLogger(
                source,
                new LogLevelStatus(config), 
                this.unityStructuredLogEventWriter);

            this.LogConfigs.Add(logger, config);

            return logger;
        }

        private ILogLevelConfig PickConfig(ILogSource source)
        {
            if (this.RegexMatchLogLevels == null)
            {
                return DefaultConfig;
            }

            for (var i = 0; i < this.RegexMatchLogLevels.Count; i++)
            {
                var spec = this.RegexMatchLogLevels[i];

                if (Regex.IsMatch(source.FullName, spec.Regex, RegexOptions.IgnoreCase))
                {
                    return spec;
                }
            }

            return DefaultConfig;
        }
    }
}
