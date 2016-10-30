namespace Andoco.Unity.Framework.Core
{
    using TickedPriorityQueue;

    public delegate void TickedActionDelegate();

    public class TickedAction : ATicked, ITickedReceipt
    {
        private readonly TickedQueue queue;
        private readonly TickedActionDelegate ticked;

        private bool cancelled;

        public TickedAction(TickedQueue queue, TickedActionDelegate ticked, int priority, double tickLength)
        {
            this.queue = queue;
            this.ticked = ticked;
            this.Priority = priority;
            this.TickLength = tickLength;
        }

        public override void OnTicked()
        {
            if (this.cancelled)
                throw new System.InvalidOperationException("Cannot tick the action because the TickedAction has been cancelled");

            this.ticked();
        }

        /// <summary>
        /// Cancel the ticked action.
        /// </summary>
        /// <remarks
        /// Once cancelled a ticked action cannot be reused.
        /// </remarks>
        public void Cancel()
        {
            if (!this.queue.Remove(this))
			{
				throw new System.InvalidOperationException("The ticked action could not be cancelled because it doesn't exist in the queue it was created for");
			}

            this.cancelled = true;
        }
    }
}