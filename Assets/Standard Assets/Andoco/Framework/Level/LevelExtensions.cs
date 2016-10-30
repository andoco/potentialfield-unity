namespace Andoco.Unity.Framework.Level
{
    using System;
    using Andoco.Unity.Framework.Level.Objectives;

    public static class LevelExtensions
    {
        public static void ResetLevelObjectives(this ILevelSystem level, string category = null)
        {
            for (int i = 0; i < level.Objectives.Count; i++)
            {
                var objective = level.Objectives[i];

                if (category == null || objective.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                {
                    objective.Reset();
                }
            }
        }
    }
}
