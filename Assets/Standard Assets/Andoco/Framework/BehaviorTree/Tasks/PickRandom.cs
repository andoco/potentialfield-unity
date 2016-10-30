namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using System.Collections.Generic;
	using System.Linq;
	
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Actions;
	
	using UnityEngine;

	public class PickRandom : ActionTask
	{
		public PickRandom(ITaskIdBuilder id)
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
			
			var selected = source[Random.Range(0, source.Length)];
			
			ctx.SetItem(this.Assign, selected);
			
			return TaskResult.Success;
		}
	}
}

