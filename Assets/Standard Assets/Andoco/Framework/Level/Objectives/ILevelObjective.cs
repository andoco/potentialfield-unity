namespace Andoco.Unity.Framework.Level.Objectives
{
    public interface ILevelObjective
    {
        string Name { get; }

        string Category { get; }
        
        bool IsRequired { get; }
        
        LevelObjectiveResult CheckObjectiveStatus();

        void Reset();
    }
}
