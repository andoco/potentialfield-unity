namespace Andoco.Unity.Framework.Input
{
    public class TargettedInputFilter : ITargettedInputFilter
    {
        public int? Layers { get; set; }

        public string NextTag { get; set; }

        public void Clear()
        {
            this.NextTag = null;
        }
    }
}