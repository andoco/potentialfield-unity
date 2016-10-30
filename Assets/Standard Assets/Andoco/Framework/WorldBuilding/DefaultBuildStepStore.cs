namespace Andoco.Unity.Framework.WorldBuilding
{
    using System;
    using System.Collections.Generic;

    public class DefaultBuildStepStore : IBuildStepStore
    {
        private List<IBuildStep> steps = new List<IBuildStep>();

        public void Add(IBuildStep buildStep)
        {
            this.steps.Add(buildStep);
        }

        public void AddAfter(string stepName, IBuildStep buildStep)
        {
            var idx = this.steps.FindIndex(step => step.Name.Equals(stepName, StringComparison.OrdinalIgnoreCase));

            if (idx > -1)
            {
                this.steps.Insert(idx + 1, buildStep);
            }
            else
            {
                this.steps.Add(buildStep);
            }
        }

        public void AddBefore(string stepName, IBuildStep buildStep)
        {
            var idx = this.steps.FindIndex(step => step.Name.Equals(stepName, StringComparison.OrdinalIgnoreCase));

            if (idx > -1)
            {
                this.steps.Insert(idx, buildStep);
            }
            else
            {
                this.steps.Add(buildStep);
            }
        }

        public IBuildStep[] GetAll()
        {
            return this.steps.ToArray();
        }
    }
}
