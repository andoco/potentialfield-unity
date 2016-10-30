namespace Andoco.Unity.Framework.BehaviorTree
{
    using System;
    using System.Collections.Generic;
    using Andoco.BehaviorTree.Reader;
    using Andoco.BehaviorTree.Reader.Source;

    /// <summary>
    /// Behavior reader selector required because Zenject only injects List<T> instead of IList<T>.
    /// </summary>
    public class UnityBehaviorReaderSelector : DefaultBehaviorReaderSelector
    {
        public UnityBehaviorReaderSelector(List<IBehaviorReader> readers, IBehaviorSourceResolver source)
            : base(readers, source)
        {
        }
    }
}