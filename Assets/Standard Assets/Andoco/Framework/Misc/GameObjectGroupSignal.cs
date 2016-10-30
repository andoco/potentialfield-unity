namespace Andoco.Unity.Framework.Misc
{
    using System;
    using System.Collections.Generic;
    using Andoco.Core.Diagnostics.Logging;
    using Andoco.Core.Signals;
    using Andoco.Unity.Framework.Core;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public class GameObjectGroupSignal : Signal<GameObjectGroupSignal.Data>
    {
        public struct Data
        {
            public string name;
            public GameObjectGroup group;
        }
    }
}
