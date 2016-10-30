namespace Andoco.Unity.Framework.Level
{
    using System.Collections.ObjectModel;
    using Andoco.Unity.Framework.Level.Monitors;
    using Andoco.Unity.Framework.Level.Objectives;
    
    public interface ILevelSystem
    {
        bool IsStarted { get; }

        bool IsPaused { get; }
    
        float LevelTime { get; }

        int LevelNumber { get; }
    
        ReadOnlyCollection<ILevelObjective> Objectives { get; }
    
        void AddObjective(ILevelObjective objective);

        void AddEvent(ILevelMonitor levelEvent);

        void AddConfig(ILevelConfig config);
    
        LevelObjectiveResult CheckRequiredObjectives(string category = null);

        T GetConfigOrDefault<T>() where T : ILevelConfig;

        void Configure(int levelNumber);
    
        void StartLevel();
    
        void PauseLevel();
    
        void ResumeLevel();
    
        void StopLevel();

        void ResetState();
    }
}
