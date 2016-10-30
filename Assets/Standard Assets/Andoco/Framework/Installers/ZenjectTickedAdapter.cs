namespace Andoco.Unity.Framework.Installers
{
    using System.Collections.Generic;
    using Andoco.Core;
    using Zenject;

    /// <summary>
    /// Adapter to allow <see cref="Andoco.Core.ITicked"/> instances to be ticked by Zenject (via ITickable).
    /// </summary>
    public class ZenjectTickedAdapter : ITickable
    {
        private readonly List<ITicked> ticked;

        public ZenjectTickedAdapter([InjectOptional] List<ITicked> ticked)
        {
            this.ticked = new List<ITicked>();

            for (int i=0; i < ticked.Count; i++)
            {
                this.ticked.Add(ticked[i]);
            }

            // TODO: Subscribe to Spawned/Recycled signals.
        }

        #region ITickable implementation

        public void Tick()
        {
            for (int i=0; i < this.ticked.Count; i++)
            {
                this.ticked[i].Tick();
            }
        }

        #endregion
    }
}
