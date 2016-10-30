namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;
    using TickedPriorityQueue;
	
    public static class TickedActionMonoBehaviourExtensions
    {
		public static ITickedReceipt ScheduleTick(this MonoBehaviour behaviour, TickConfig config, TickedActionDelegate ticked)
		{
			return behaviour.ScheduleTick(config.queueName, ticked, config.tickPriority, config.tickLength);
		}

        public static ITickedReceipt ScheduleTick(this MonoBehaviour behaviour, string queueName, TickedActionDelegate ticked, int priority, float tickLength)
        {
            var queue = UnityTickedQueue.GetInstance(queueName);
            var action = new TickedAction(queue.Queue, ticked, priority, (double)tickLength);
            queue.Add(action);

            return action;
        }

        public static ITickedReceipt ScheduleTick(this MonoBehaviour behaviour, TickedActionDelegate ticked, int priority, float tickLength)
        {
            return ScheduleTick(null, ticked, priority, tickLength);
        }
    }
}