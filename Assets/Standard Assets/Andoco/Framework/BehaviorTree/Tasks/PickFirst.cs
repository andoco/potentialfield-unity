namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using System.Collections.Generic;
	using System.Linq;
	
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Actions;
	
	using UnityEngine;

	public class PickFirst : ActionTask
	{
		public PickFirst(ITaskIdBuilder id)
			: base(id)
		{
		}
		
		public string Source { get; set; }
		
		public string Assign { get; set; }
		
		public override TaskResult Run(ITaskNode node)
		{
			var ctx = node.Context;
			var source = ctx.GetGameObjects(this.Source).ToArray();
			
			if (!source.Any())
				return TaskResult.Failure;
			
			var selected = source.First();
			
			ctx.SetItem(this.Assign, selected);
			
			return TaskResult.Success;
		}
	}
}

