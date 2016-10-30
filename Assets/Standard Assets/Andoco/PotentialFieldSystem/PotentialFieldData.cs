using System;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class PotentialFieldData : ScriptableObject
    {
        [Tooltip("A key that uniquely identifies this instance")]
        public string datasetKey = "Default";

        public LayerData[] layers;

        [Serializable]
        public class LayerData
        {
            public string name;
            public int value;
			public bool flowField;
        }
    }
}
