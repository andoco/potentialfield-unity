namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class UnityLoggerFactoryV2 : ILoggerFactory
    {
        private const string GameObjectFieldKey = "gameObject";
        private const string EntityFieldKey = "entity";

        private static Logging rootLogConfig;

        private readonly GameObject gameobject;
        private readonly ILogLevelConfig config;
        private readonly IStructuredLogEventWriter writer;

        public UnityLoggerFactoryV2([Zenject.InjectOptional] GameObject gameobject, IStructuredLogEventWriter writer)
        {
            this.gameobject = gameobject;
            this.writer = writer;

            this.config = this.GetConfig(gameobject);

            Assert.IsNotNull(this.config);
        }

        private Logging GetConfig(GameObject gameobject)
        {
            // Get the Logging config from own gameobject or any parent.
            Logging config = null;
            if (gameobject != null)
                config = gameobject.GetComponentInParent<Logging>();

            if (config == null)
            {
                if (rootLogConfig == null)
                {
                    var rootConfigGo = GameObject.Find("Logging");

                    if (rootConfigGo == null)
                    {
                        rootConfigGo = new GameObject("Logging");
                    }

                    config = rootConfigGo.GetComponent<Logging>();

                    if (config == null)
                    {
                        config = rootConfigGo.AddComponent<Logging>();
                    }

                    rootLogConfig = config;
                }
                else
                {
                    config = rootLogConfig;
                }
            }

            return config;
        }

        public IStandardLog CreateLogger(ILogSource source)
        {
            var logger = new UnityLogger(source, this.config);

            return logger;
        }

        public IStructuredLog CreateStructuredLog(ILogSource source)
        {
            Assert.IsNotNull(source);

            var log = new StructuredLogger(source, new LogLevelStatus(this.config), this.writer);

            if (this.gameobject != null)
            {
                log.DefaultFields.Add(new KeyValuePair<string, object>(GameObjectFieldKey, this.gameobject));

                var entityGo = this.gameobject.GetEntityGameObject();
                if (entityGo != null)
                {
                    log.DefaultFields.Add(new KeyValuePair<string, object>(EntityFieldKey, entityGo));
                }
            }

            return log;
        }
    }
}
