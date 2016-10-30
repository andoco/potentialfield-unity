namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LoadScene : ActionTask
    {
        public LoadScene(ITaskIdBuilder id)
            : base(id)
        {
            this.SceneIndex = -1;
        }

        public string SceneName { get; set; }

        public int SceneIndex { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
            if (!string.IsNullOrEmpty(this.SceneName))
            {
                SceneManager.LoadScene(this.SceneName);
            }
            else if (this.SceneIndex >= 0)
            {
                SceneManager.LoadScene(this.SceneIndex);
            }
            else
            {
                throw new InvalidOperationException("Cannot load scene. No SceneName or SceneIndex specified");
            }

            return TaskResult.Success;
        }
    }
}
