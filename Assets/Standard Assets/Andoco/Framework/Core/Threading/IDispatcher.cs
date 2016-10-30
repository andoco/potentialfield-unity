namespace Andoco.Unity.Framework.Core
{
    using System;

    public interface IDispatcher
    {
        void Schedule(Action action);
    }
}
