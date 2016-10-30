namespace Andoco.Unity.Framework.Weighting
{
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Andoco.Core;
    using Andoco.Unity.Framework.Core;

    public interface IWeightCollector
    {
        string Category { get; }
        
        void FillWeights(IList<WeightedItem> weights);
    }
    
}