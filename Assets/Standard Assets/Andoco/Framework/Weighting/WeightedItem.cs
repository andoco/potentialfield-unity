using System;

namespace Andoco.Unity.Framework.Weighting
{
    using UnityEngine;
    using System.Linq;
    using System.Collections.Generic;
    using Andoco.Core;
    using Andoco.Unity.Framework.Movement.Objectives;

    public sealed class WeightedItem : System.IComparable<WeightedItem>
    {
        public WeightedItem()
        {
        }

        public WeightedItem(WeightedItem item)
        {
            this.Target = item.Target;
            this.TargetObject = item.TargetObject;
            this.Weight = item.Weight;
        }

		public Objective Target { get; set; }

        public object TargetObject { get; set; }
        
        public float Weight { get; set; }

        public int CompareTo(WeightedItem other)
        {
            // Sort in descending order
            return this.Weight == other.Weight ? 0 : (this.Weight < other.Weight ? 1 : -1);
        }
    }
}