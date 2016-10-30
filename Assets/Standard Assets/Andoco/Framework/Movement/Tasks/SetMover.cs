using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using Andoco.Unity.Framework.Movement.Movers;

namespace Andoco.Unity.Framework.Movement.Tasks
{
	public class SetMover : ActionTask
	{
		public SetMover(ITaskIdBuilder id)
			: base(id)
		{
		}

		public string Driver { get; set; }

		public string Navigator { get; set; }

		public override TaskResult Run(ITaskNode node)
		{
			var mover = node.Context.GetGameObject().GetComponent<Mover>();

			if (!string.IsNullOrEmpty(this.Driver))
			{
				mover.SetDriver(this.Driver);
			}

			if (!string.IsNullOrEmpty(this.Navigator))
			{
				mover.SetNavigator(this.Navigator);
			}

			return TaskResult.Success;
		}
	}
}
