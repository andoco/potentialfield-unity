namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using System;
	using System.Linq;
	
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Conditions;
	using Andoco.Unity.Framework.Core;
	
	using UnityEngine;

	public class TagEval : Condition
	{
		public TagEval(ITaskIdBuilder id)
			: base(id)
		{
		}

		public string Target { get; set; }
		
		public string Set { get; set; }
		
		public string Unset { get; set; }

		public override TaskResult Run (ITaskNode node)
		{
			var ctx = node.Context;
			var targets = ctx.GetGameObjects(this.Target);

			if (!targets.Any())
				return TaskResult.Failure;

			var setTags = string.IsNullOrEmpty(this.Set)
				? new string[0]
				: this.Set.Split(',').Select(tag => tag.Trim()).ToArray();
			
			var unsetTags = string.IsNullOrEmpty(this.Unset)
				? new string[0]
				: this.Unset.Split(',').Select(tag => tag.Trim()).ToArray();
			
			foreach (var t in targets)
			{
				if (t == null)
					throw new InvalidOperationException(string.Format("Cannot evaluate tags (set=[{0}], unset=[{1}]. Target has already been destroyed", this.Set, this.Unset));

				var multitags = t.GetComponent<MultiTags>();
				
				if (multitags == null)
					throw new InvalidOperationException(string.Format("Target gameobject {0} does not have a MultiTags component", t));
				
				foreach (var tag in setTags)
				{
					if (!multitags.HasTag(tag))
						return TaskResult.Failure;
				}
				
				foreach (var tag in unsetTags)
				{
					if (multitags.HasTag(tag))
						return TaskResult.Failure;
				}
			}
			
			return TaskResult.Success;
		}
	}
}
