namespace Andoco.Unity.Framework.WorldBuilding
{
    public interface IBuildStepStore
    {
        void Add(IBuildStep buildStep);

        void AddAfter(string stepName, IBuildStep buildStep);

        void AddBefore(string stepName, IBuildStep buildStep);

        IBuildStep[] GetAll();
    }
}
