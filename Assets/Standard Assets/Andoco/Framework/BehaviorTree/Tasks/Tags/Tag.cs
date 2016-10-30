namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Actions;
	using Andoco.Unity.Framework.Core;
	
	using UnityEngine;

	public class Tag : ActionTask
	{		
	    public Tag(ITaskIdBuilder id)
	        : base(id)
	    {
	    }
		
		public string Target { get; set; }
		
		public string Set { get; set; }
		
		public string Unset { get; set; }
		
		public override TaskResult Run(ITaskNode node)
		{
			var ctx = node.Context;
			var targets = ctx.GetGameObjects(this.Target);
			
            var tagsToSet = MultiTags.Parse(this.Set);
            var tagsToUnset = MultiTags.Parse(this.Unset);
			
			foreach (var t in targets)
			{
				var multitags = t.GetComponent<MultiTags>();
				
				if (multitags == null)
					throw new InvalidOperationException(string.Format("Target gameobject {0} does not have a MultiTags component", t));
				
				foreach (var tag in tagsToSet)
					multitags.AddTag(tag);
				
				foreach (var tag in tagsToUnset)
					multitags.RemoveTag(tag);
			}
			
			return TaskResult.Success;			
		}		
	}
}

