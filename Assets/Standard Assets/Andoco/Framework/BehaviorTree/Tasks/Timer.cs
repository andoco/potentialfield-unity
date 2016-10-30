using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using UnityEngine;

namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	public class Timer : ActionTask
	{		
		private readonly string targetTimeKey;

	    public Timer(ITaskIdBuilder id)
	        : base(id)
	    {
			this.targetTimeKey = this.PrefixKey("targetTime");
	    }

        public ItemProperty<float> Time { get; set; }
        
        public ItemProperty<float> Variance { get; set; }
		
		protected override void Starting(ITaskNode node)
		{
			var ctx = node.Context;
			var time = this.Time.TryGetValue(ctx, 1);
            var variance = this.Variance.TryGetValue(ctx, 0);

			var t = variance > 0 ?
				time + (UnityEngine.Random.value * -variance * 2) :
				time;
			
			var targetTime = UnityEngine.Time.time + t;
			ctx.SetItem(this.targetTimeKey, targetTime);
		}
	
		public override TaskResult Run(ITaskNode node)
		{
			var targetTime = node.Context.GetItem<float>(this.targetTimeKey);
			
			return targetTime <= UnityEngine.Time.time ? TaskResult.Success : TaskResult.Pending;
		}
	}
}

