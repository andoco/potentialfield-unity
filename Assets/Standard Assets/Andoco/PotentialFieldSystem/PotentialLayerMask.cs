namespace Andoco.Unity.Framework.PotentialField
{
    [System.Serializable]
    public struct PotentialLayerMask
    {
        public string datasetKey;
        public int value;

        public static implicit operator int(PotentialLayerMask layer)
        {
            return layer.value;
        }

        public override string ToString()
        {
            return string.Format("[PotentialLayerMask: value={0}]", value);
        }
    }
}
