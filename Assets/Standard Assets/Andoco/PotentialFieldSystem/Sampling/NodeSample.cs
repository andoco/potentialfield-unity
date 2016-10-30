namespace Andoco.Unity.Framework.PotentialField
{
	public struct NodeSample
	{
		public NodeSample(IFieldNodeRef node, float potential)
		{
			this.Node = node;
			this.Potential = potential;
		}

		public IFieldNodeRef Node { get; private set; }

		public float Potential { get; private set; }
	}
}
