namespace Andoco.Unity.Framework.Level.Monitors
{
    using System;
    using Andoco.Unity.Framework.Core;

    public interface ILevelMonitorConfig : INamedConfig
    {
        string Category { get; }
    }
}
