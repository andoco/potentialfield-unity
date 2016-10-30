namespace Andoco.Unity.Framework.Core.Logging
{
    using System;
    using UnityEngine;

    [Serializable]
    public class RegexMatchLogLevelConfig : SerializableLogLevelConfig
    {
        [SerializeField]
        private string regex;
        
        public string Regex
        {
            get { return this.regex; }
            set { this.regex = value; }
        }
    }
}