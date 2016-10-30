namespace Andoco.Unity.Framework.Level.Monitors
{
    using System;

    [System.Serializable]
    public class LevelMonitorConfig : ILevelMonitorConfig
    {
        public string name;

        public string category;

        public string Name { get { return this.name; } }

        public string Category { get { return this.category; } }
    }
}
