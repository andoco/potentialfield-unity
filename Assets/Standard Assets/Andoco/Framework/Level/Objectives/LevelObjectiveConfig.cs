namespace Andoco.Unity.Framework.Level.Objectives
{
    public abstract class LevelObjectiveConfig : ILevelObjectiveConfig
    {
        public string name;

        public string category;
    
        public bool isRequired;
        
        public string Name { get { return this.name; } }

        public string Category { get { return this.category; } }
    
        public bool IsRequired { get { return this.isRequired; } }
    }
}
