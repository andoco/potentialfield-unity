using System;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    [System.Serializable]
    public struct NodeSourceConf
    {
        public bool enabled;
        public PotentialLayerMask layers;
        public float potential;
        public Component calculator;
        public string sourceKey;
		public PotentialFlowKind flow;
    }
}
