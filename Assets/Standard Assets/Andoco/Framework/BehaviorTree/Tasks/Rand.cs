namespace Andoco.Unity.Framework.BehaviorTree.Tasks
{
	using Andoco.BehaviorTree;
	using Andoco.BehaviorTree.Conditions;
	using UnityEngine;

	public class Rand : Condition
	{
		public float Pass { get; set; }
		
		public Rand(ITaskIdBuilder id)
			: base(id)
		{
		}

        protected override bool Evaluate(ITaskNode node)
		{
			return Random.value >= this.Pass;
		}
	}
}
