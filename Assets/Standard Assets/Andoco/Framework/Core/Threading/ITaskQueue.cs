namespace Andoco.Unity.Framework.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Andoco.Core;
    using UnityEngine;

    public interface ITaskQueue
    {
        void Start();

        void Stop();

        void Schedule(Action work, Action callback = null);
    }
    
}
