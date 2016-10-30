namespace Andoco.Unity.Framework.Data
{
    public interface IGameData
    {
        void Set<T>(string key, T data);

        T Get<T>(string key);

        T GetOrAdd<T>(string key) where T : class, new();

        bool Has(string key);
    }
}
