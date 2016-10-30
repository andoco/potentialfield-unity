namespace Andoco.Unity.Framework.Level.Objectives
{
    using Andoco.Unity.Framework.Core;
    
    public interface ILevelObjectiveConfig : INamedConfig
    {
        string Category { get; }

        bool IsRequired { get; }
    }
}
