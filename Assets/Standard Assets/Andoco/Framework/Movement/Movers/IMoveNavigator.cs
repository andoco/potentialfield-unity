namespace Andoco.Unity.Framework.Movement.Movers
{
    /// <summary>
    /// An <see cref="IMoveNavigator"/> is responsible for providing a <see cref="Mover"/> with destinations.
    /// </summary>
    public interface IMoveNavigator
    {
        string Name { get; }

        void StartNavigating();

        void StopNavigating();
    }
}