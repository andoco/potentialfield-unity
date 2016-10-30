namespace Andoco.Unity.Framework.Core
{
    using UnityEngine;
    using System;

    [Serializable]
    public class TickedQueueConfig
    {
        public string name;
        public int maxProcessedPerUpdate;
        public float maxProcessingTimePerUpdate;
    }

    public class UnityTickedQueueController : MonoBehaviour {

        public TickedQueueConfig[] queues;

    	void Start()
        {
            if (this.queues != null)
            {
                foreach (var qc in this.queues)
                {
                    var q = UnityTickedQueue.GetInstance(qc.name).Queue;
                    q.MaxProcessedPerUpdate = qc.maxProcessedPerUpdate;
                    q.MaxProcessingTimePerUpdate = qc.maxProcessingTimePerUpdate;
                }
            }
    	}    	
    }
}