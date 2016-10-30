namespace Andoco.Unity.Framework.Input
{
    public interface ITargettedInputFilter
    {
        int? Layers { get; set; }

        string NextTag { get; set; }

        void Clear();
    }
}
