using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Andoco.Core.Pooling;
using Andoco.Unity.Framework.Core;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
    public class Layer
    {
        public Layer(int layerNum, PotentialLayerKind layerKind)
        {
            this.LayerNum = layerNum;
            this.Kind = layerKind;
            this.Sources = new List<PotentialFieldNodeSource>();
        }

        public int LayerNum { get; private set; }

        public int LayerMask { get { return 1 << this.LayerNum; } }

        public PotentialLayerKind Kind { get; private set; }

        public List<PotentialFieldNodeSource> Sources { get; private set; }

        public bool IsOneOfLayers(int layerMask)
        {
            return (this.LayerMask & layerMask) == this.LayerMask;
        }
    }
    
}
