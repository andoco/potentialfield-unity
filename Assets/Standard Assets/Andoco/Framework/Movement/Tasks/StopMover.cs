using Andoco.BehaviorTree;
using Andoco.BehaviorTree.Actions;
using Andoco.Unity.Framework.BehaviorTree;
using Andoco.Unity.Framework.Movement.Movers;

namespace Andoco.Unity.Framework.Movement.Tasks
{
	public class StopMover : ActionTask
	{
		public StopMover(ITaskIdBuilder id)
			: base(id)
		{
		}

		public bool Driver { get; set; }

		public bool Navigator { get; set; }

		public override TaskResult Run(ITaskNode node)
		{
			var mover = node.Context.GetGameObject().GetComponent<Mover>();

			if (this.Navigator)
			{
				mover.Navigator.StopNavigating();
			}

			if (this.Driver)
			{
				mover.Driver.Cancel();
			}

			return TaskResult.Success;
		}
	}
}
