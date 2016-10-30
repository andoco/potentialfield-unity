namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using UnityEngine;
    using Andoco.Core.Diagnostics.Logging;

    [Serializable]
    public class SerializableLogLevelConfig : LogLevelConfig
    {
        [SerializeField]
        private bool trace;

        [SerializeField]
        private bool debug;

        [SerializeField]
        private bool info;

        [SerializeField]
        private bool warning;

        [SerializeField]
        private bool error;

        [SerializeField]
        private bool fatal;

        public override bool Debug
        {
            get { return this.debug; }
            set { this.debug = value; }
        }

        public override bool Trace
        {
            get { return this.trace; }
            set { this.trace = value; }
        }

        public override bool Info
        {
            get { return this.info; }
            set { this.info = value; }
        }

        public override bool Warning
        {
            get { return this.warning; }
            set { this.warning = value; }
        }

        public override bool Error
        {
            get { return this.error; }
            set { this.error = value; }
        }

        public override bool Fatal
        {
            get { return this.fatal; }
            set { this.fatal = value; }
        }
    }
    
}