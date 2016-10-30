namespace Andoco.Unity.Framework.Weighting
{
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Andoco.Core;
    using Andoco.Unity.Framework.Core;
    
    public class ActionWeightCollector : IWeightCollector
    {
        private Action<IList<WeightedItem>> collect;
        
        public ActionWeightCollector(string category, Action<IList<WeightedItem>> collect)
        {
            this.Category = category;
            this.collect = collect;
        }
        
        public string Category { get; private set; }
        
        public void FillWeights(IList<WeightedItem> weights)
        {
            this.collect(weights);
        }
    }
    
}