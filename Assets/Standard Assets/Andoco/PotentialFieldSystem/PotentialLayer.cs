using System;

namespace Andoco.Unity.Framework.PotentialField
{
    [System.Serializable]
    public struct PotentialLayer
    {
        public string datasetKey;
        public int value;

        public static implicit operator int(PotentialLayer layer)
        {
            return layer.value;
        }

        public int MaskValue { get { return 1 << value; } }

        public override string ToString()
        {
            return string.Format("[PotentialLayer: value={0}]", value);
        }
    }
}
