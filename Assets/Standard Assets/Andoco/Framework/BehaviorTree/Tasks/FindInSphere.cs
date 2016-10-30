namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Actions;
	
	using UnityEngine;

	public class FindInSphere : ActionTask
	{
		public FindInSphere(ITaskIdBuilder id)
			: base(id)
		{
		}

		public string Origin { get; set; }

		public ItemProperty<float> Radius { get; set; }

		public string Assign { get; set; }

		public override TaskResult Run (ITaskNode node)
		{
			var ctx = node.Context;
			var originGo = ctx.GetGameObject(this.Origin);
			var originPos = originGo.transform.position;

			var foundColliders = Physics.OverlapSphere(originPos, this.Radius.GetValue(ctx));
			var foundGos = foundColliders.Select(c => c.gameObject).ToArray();

			//UnityEngine.Debug.Log(string.Format("Found {0} gameobjects", foundGos.Length));

			ctx.SetItem(this.Assign, foundGos);

			return foundGos.Any() ? TaskResult.Success : TaskResult.Failure;
		}
	}
}

