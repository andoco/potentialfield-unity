namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using System.Collections.Generic;
	using System.Linq;
	
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Actions;
	
	using UnityEngine;

	public class PickClosest : ActionTask
	{
		public PickClosest(ITaskIdBuilder id)
			: base(id)
		{
		}

		public string Target { get; set; }
		
		public string Source { get; set; }
		
		public string Assign { get; set; }
		
		public override TaskResult Run(ITaskNode node)
		{
			var ctx = node.Context;
			var go = ctx.GetGameObject(this.Target);
			var source = ctx.GetItem<IEnumerable<GameObject>>(this.Source).ToArray();
			
			if (!source.Any())
				return TaskResult.Failure;

			GameObject closest = null;
			float closestDist = float.MaxValue;

			foreach (var srcGo in source)
			{
				var d = Vector3.Distance(go.transform.position, srcGo.transform.position);
				if (d < closestDist)
				{
					closest = srcGo;
					closestDist = d;
				}
			}

			if (closest == null)
				throw new System.InvalidOperationException("Closest gameobject not found");

			ctx.SetItem(this.Assign, closest);
			
			return TaskResult.Success;
		}
	}
}

