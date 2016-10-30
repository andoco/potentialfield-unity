namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Actions;
	using UnityEngine;
	
    public class FindGameObjects : ActionTask
    {
        public FindGameObjects(ITaskIdBuilder id)
            : base(id)
        {
        }

        public string Tag { get; set; }

        public string Name { get; set; }

        public string Assign { get; set; }

        public override TaskResult Run(ITaskNode node)
        {
			var ctx = node.Context;
        	if (!string.IsNullOrEmpty(this.Name))
        		ctx.SetItem(this.Assign, GameObject.Find(this.Name));
        	else if (!string.IsNullOrEmpty(this.Tag))
        		ctx.SetItem(this.Assign, GameObject.FindGameObjectsWithTag(this.Tag));

        	return TaskResult.Success;
        }
    }
}
