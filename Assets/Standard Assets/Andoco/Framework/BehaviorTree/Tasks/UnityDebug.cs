namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Actions;
	using Andoco.Unity.Framework.BehaviorTree;

#if UNITY_EDITOR
    using UnityEditor;
#endif
    using UnityEngine;
    
    public class UnityDebug : ActionTask
    {
        public UnityDebug(ITaskIdBuilder id)
            : base(id)
        {
        }
        
        public string Info { get; set; }
        
        public string Warn { get; set; }
        
        public string Error { get; set; }
        
        public bool Select { get; set; }
        
        public bool Break { get; set; }
        
        public override TaskResult Run(ITaskNode node)
        {
			var ctx = node.Context;
            var go = ctx.GetGameObject();
            
            if (!string.IsNullOrEmpty(this.Info))
                UnityEngine.Debug.Log(FormatMsg(this.Info, go), go);
            
            if (!string.IsNullOrEmpty(this.Warn))
                UnityEngine.Debug.LogWarning(FormatMsg(this.Warn, go), go);
            
            if (!string.IsNullOrEmpty(this.Error))
                UnityEngine.Debug.LogError(FormatMsg(this.Error, go), go);

#if UNITY_EDITOR
            if (this.Select)
            {
                Selection.activeGameObject = go;
            }
#endif
            
            if (this.Break)
            {
                //UnityEngine.Debug.Log(string.Format("Breaking in task {0}", this));
                UnityEngine.Debug.Break();
            }
            
            return TaskResult.Success;
        }

        private static string FormatMsg(string msg, GameObject go)
        {
            return string.Format("{0} {1} {2}", Time.time, go, msg);
        }
    }
}