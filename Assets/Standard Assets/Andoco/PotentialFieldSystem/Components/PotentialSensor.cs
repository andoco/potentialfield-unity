using System;
using System.Collections.Generic;
using Andoco.BehaviorTree.Signals;
using UnityEngine;
using Zenject;

namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialSensor : MonoBehaviour
    {
        IFieldNodeRef currentNode = null;

        [SerializeField]
        Entry[] entries;

        [Inject]
        IPotentialFieldSystem potentialSys;

        [Inject]
        FlagSignal signal;

        [Inject]
        void OnPostInject()
        {
        }

        void Update()
        {
            if (potentialSys == null)
                return;

            currentNode = potentialSys.GetClosestNode(transform.position, currentNode);

            for (int i = 0; i < entries.Length; i++)
            {
                var req = new SampleRequest
                {
                    nodes = new List<IFieldNodeRef> { currentNode },
                    potentialLayerMask = entries[i].layers
                };

                var potentials = potentialSys.SamplePotential(req);

                if (Evaluate(potentials[0], entries[i].comparison, entries[i].value))
                {
                    signal.DispatchFlag(gameObject, entries[i].flag);
                }
            }
        }

        bool Evaluate(float potential, Op op, float value)
        {
            switch (op)
            {
                case Op.Gt:
                    return potential > value;
                case Op.Gte:
                    return potential >= value;
                case Op.Lt:
                    return potential < value;
                case Op.Lte:
                    return potential <= value;
                default:
                    return false;
            }
        }

        [Serializable]
        struct Entry
        {
            public PotentialLayerMask layers;
            public Op comparison;
            public float value;
            public string flag;
        }

        enum Op
        {
            None,
            Gte,
            Gt,
            Lte,
            Lt
        }
    }
}