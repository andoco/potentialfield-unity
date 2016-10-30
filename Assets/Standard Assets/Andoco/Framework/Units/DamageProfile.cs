namespace Andoco.Unity.Framework.Units
{
    [System.Serializable]
    public struct DamageProfile
    {
        public string key;
        public DamageKind mode;
        public float amount;
        public float frequency;
    }
}
