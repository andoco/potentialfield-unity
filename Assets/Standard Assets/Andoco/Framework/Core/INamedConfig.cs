namespace Andoco.Unity.Framework.Core
{
    /// <summary>
    /// Common interface for all configuration types that require a name to identify the object being configured.
    /// </summary>
    /// <remarks>
    /// This is mostly to help with auto-discovery of configuration objects in the scene.
    /// </remarks>
    public interface INamedConfig : IConfig
    {
        string Name { get; }
    }
}