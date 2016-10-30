namespace Andoco.Unity.Framework.Level.Objectives
{
    public abstract class LevelObjective : ILevelObjective
    {
        protected LevelObjective()
        {
            Name = GetType().Name;
            Category = "Level";
        }

        public string Name { get; protected set; }

        public string Category { get; protected set; }

        public bool IsRequired { get; protected set; }

        public abstract LevelObjectiveResult CheckObjectiveStatus();

        public virtual void Reset() { }
    }
}
