namespace Andoco.Unity.Framework.Core
{
	[System.Serializable]
	public class TickConfig
	{
		public TickConfig()
		{
			this.queueName = "Default";
			this.tickPriority = 1;
			this.tickLength = 1f;
		}

		public string queueName;
		public int tickPriority;
		public float tickLength;
	}
}