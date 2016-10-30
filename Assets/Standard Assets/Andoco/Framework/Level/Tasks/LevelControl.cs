namespace Andoco.Unity.Framework.Level.Tasks
{
    using System;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
    using Andoco.Unity.Framework.BehaviorTree;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;
    
    public class LevelControl : ActionTask
    {
        public LevelControl(ITaskIdBuilder id)
            : base(id)
        {
        }
    
        public ControlKind Action { get; set; }
    
        public override TaskResult Run(ITaskNode node)
        {
            Debug.LogFormat("LevelControl: {0}", this.Action);

            var level = Indexed.GetSingle<ILevelSystem>();
    
            switch (this.Action)
            {
                case ControlKind.Pause:
                    level.PauseLevel();
                    break;
                case ControlKind.Resume:
                    level.ResumeLevel();
                    break;
                case ControlKind.Start:
                    level.StartLevel();
                    break;
                case ControlKind.Stop:
                    level.StopLevel();
                    break;
                case ControlKind.ResetState:
                    level.ResetState();
                    break;
                default:
                    throw new InvalidOperationException(string.Format("Unsupported {0} value {1}", typeof(ControlKind), this.Action));
            }
    
            return TaskResult.Success;
        }
    
        #region Types
    
        public enum ControlKind
        {
            None,
            Pause,
            ResetState,
            Resume,
            Start,
            Stop
        }
    
        #endregion
    }
}
