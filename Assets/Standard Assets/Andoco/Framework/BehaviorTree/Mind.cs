namespace Andoco.Unity.Framework.BehaviorTree
{
    using Andoco.Unity.Framework.Core;
    using UnityEngine;

    public class Mind : MonoBehaviour
    {
        public int IterationRate = 10;

        public string BehaviorFile;

        public bool autoStart = true;

        void Start()
        {
            Indexed.GetSingle<IMindSystem>().Add(this);
        }

        void OnDestroy()
        {
            var mindSys = Indexed.GetSingleOrDefault<IMindSystem>();

            if (mindSys != null)
                mindSys.Remove(this);
        }

        void Spawned()
        {
            Indexed.GetSingle<IMindSystem>().StartMind(this);
        }

        void Recycled()
        {
            Indexed.GetSingle<IMindSystem>().StopMind(this);
        }
    }
}