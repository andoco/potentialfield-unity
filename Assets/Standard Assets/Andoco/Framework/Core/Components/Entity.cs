using Andoco.Core.Signals;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.Core
{
    public class EntitySignal : Signal<EntitySignal.Data>
    {
        public struct Data
        {
            public Entity entity;
            public bool recycled;
            public bool destroyed;

            public bool RecycledOrDestroyed
            {
                get
                {
                    return this.recycled || this.destroyed;
                }
            }
        }
    }

    public class Entity : MonoBehaviour
    {
        [Inject]
        private EntitySignal signal;

        [ReadOnly(RuntimeOnly=true)]
        public string kind;

        [ReadOnly(RuntimeOnly=true)]
        public string label;

        [SerializeField]
        private bool signalRecycled = true;

        [SerializeField]
        private bool signalDestroyed = true;

        void Recycled()
        {
            if (this.signalRecycled && this.signal != null)
                this.signal.Dispatch(new EntitySignal.Data { entity = this, recycled = true });
        }

        void OnDestroy()
        {
            if (this.signalDestroyed && this.signal != null)
                this.signal.Dispatch(new EntitySignal.Data { entity = this, destroyed = true });
        }

        public bool IsKind(string entityKind)
        {
            return string.Equals(this.kind, entityKind, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}