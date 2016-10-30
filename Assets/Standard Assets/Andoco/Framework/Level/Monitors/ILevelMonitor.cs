namespace Andoco.Unity.Framework.Level.Monitors
{
    using System;

    public interface ILevelMonitor
    {
        string Name { get; }

        string Category { get; }

        void ResetMonitor();

        void StartMonitor();

        void StopMonitor();
    }
}
