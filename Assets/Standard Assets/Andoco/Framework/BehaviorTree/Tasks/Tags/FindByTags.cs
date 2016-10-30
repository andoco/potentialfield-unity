namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using System.Collections.Generic;
	using System.Linq;
	
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Actions;
	using Andoco.Unity.Framework.Core;
	
	using UnityEngine;
	
	public class FindByTags : ActionTask
	{
		public FindByTags(ITaskIdBuilder id)
			: base(id)
		{
		}

        public string Require { get; set; }
		
		public string Include { get; set; }
		
		public string Exclude { get; set; }
		
		public string Assign { get; set; }
		
		public override TaskResult Run(ITaskNode node)
		{
			var ctx = node.Context;
            var require = MultiTags.Parse(this.Require);
			var include = MultiTags.Parse(this.Include);
			var exclude = MultiTags.Parse(this.Exclude);

			var filtered = MultiTags.FindGameObjectsWithTags(require, include, exclude);

			ctx.SetItem(this.Assign, filtered);
			
			return TaskResult.Success;
		}
	}
}

