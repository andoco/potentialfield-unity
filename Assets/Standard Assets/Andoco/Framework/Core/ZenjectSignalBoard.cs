namespace Andoco.Unity.Framework.Core
{
    using Andoco.Core.Signals;
    using Zenject;

    public class ZenjectSignalBoard : ISignalBoard
    {
        readonly DiContainer container;

        public ZenjectSignalBoard(DiContainer container)
        {
            this.container = container;
        }

        public TSignal Get<TSignal>() where TSignal : ISignal
        {
            return container.Resolve<TSignal>();
        }
    }
}
