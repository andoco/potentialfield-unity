namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialFieldNodeSource
    {
        public PotentialFieldNodeSource(object context, string sourceKey, int layers)
        {
            this.Context = context;
            this.SourceKey = sourceKey;
            this.Layers = layers;
        }

        public object Context { get; private set; }
        public string SourceKey { get; private set; }
        public int Layers { get; private set; }
        public float Potential { get; set; }
        public IPotentialCalculator Calculator { get; set; }
        public IFieldNodeRef Node { get; set; }
        public bool Enabled { get; set; }
		public PotentialFlowKind Flow { get; set; }
    }
}
