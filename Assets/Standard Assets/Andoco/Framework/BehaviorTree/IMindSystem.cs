namespace Andoco.Unity.Framework.BehaviorTree
{
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.BehaviorTree;
    using Andoco.BehaviorTree.Scheduler;
    using Andoco.Core;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Zenject;

    public interface IMindSystem
    {
        void Add(Mind mind);

        void StartMind(Mind mind);

        void StopMind(Mind mind);

        void Remove(Mind mind);
    }
    
}