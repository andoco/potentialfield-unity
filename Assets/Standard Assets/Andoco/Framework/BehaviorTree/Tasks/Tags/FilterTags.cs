namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using System.Collections.Generic;
	using System.Linq;
	
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Actions;
	using Andoco.Unity.Framework.Core;
	
	using UnityEngine;

	public class FilterTags : ActionTask
	{
		public FilterTags(ITaskIdBuilder id)
			: base(id)
		{
		}
		
        /// <summary>
        /// GameObjects to filter.
        /// </summary>
        /// <value>The source.</value>
		public string Source { get; set; }

        /// <summary>
        /// GameObject must have all of the tags to pass the filter.
        /// </summary>
        /// <value>The require.</value>
        public string Require { get; set; }
		
        /// <summary>
        /// GameObject must have any of the tags to pass the filter.
        /// </summary>
        /// <value>The include.</value>
		public string Include { get; set; }
		
        /// <summary>
        /// GameObject must have none of the tags to pass the filter.
        /// </summary>
        /// <value>The exclude.</value>
		public string Exclude { get; set; }
		
        /// <summary>
        /// Context item to assign the filtered objects to.
        /// </summary>
        /// <value>The assign.</value>
		public string Assign { get; set; }
		
		public override TaskResult Run(ITaskNode node)
		{
			var ctx = node.Context;
			var source = ctx.GetGameObjects(this.Source);
            var require = MultiTags.Parse(this.Require);
			var include = MultiTags.Parse(this.Include);
			var exclude = MultiTags.Parse(this.Exclude);
			
            var filtered = MultiTags.FindGameObjectsWithTags(source, require, include, exclude);
			
			ctx.SetItem(this.Assign, filtered);
			
			return TaskResult.Success;
		}
	}
}

