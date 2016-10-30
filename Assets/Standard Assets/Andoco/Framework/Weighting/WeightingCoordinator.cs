using System;

namespace Andoco.Unity.Framework.Weighting
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Andoco.Unity.Framework.Core;
    
    public class WeightingCoordinator : MonoBehaviour {
        
        public static WeightingCoordinator Instance { get; private set; }
        
        private IList<WeightCalculator> weightCalculators;
        private float gizmoLastUpdated;
        private IDictionary<string, List<WeightedItem>> cachedGizmoWeights;
        
        public bool loggingEnabled;

        public bool drawGizmos;
        public bool drawTopWeightGizmo;
        public string gizmoCategories;
        public float gizmoUpdateInterval = 1f;
        public float gizmoMaxWeight = 1f;
        public float gizmoMaxHeight = 10f;
        public float gizmoMaxSize = 1f;
        
        void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            this.weightCalculators = this.gameObject.GetComponents<WeightCalculator>();
            this.Log(string.Format("Found {0} WeightCalculators: {1}", this.weightCalculators.Count, string.Join(", ", this.weightCalculators.Select(x => x.weightName).ToArray())), this.loggingEnabled);
        }

        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                if (this.drawGizmos && this.gizmoCategories != null)
                {
                    if (this.cachedGizmoWeights == null)
                        this.cachedGizmoWeights = new Dictionary<string, List<WeightedItem>>();

                    var categories = this.gizmoCategories.Split(',');

                    foreach (var category in categories)
                    {
                        // Get the cached gizmo weights.
                        List<WeightedItem> weights;
                        if (!this.cachedGizmoWeights.TryGetValue(category, out weights))
                        {
                            weights = new List<WeightedItem>();
                            this.cachedGizmoWeights.Add(category, weights);
                        }

                        // Update the cached gizmo weights.
                        if (this.gizmoLastUpdated + this.gizmoUpdateInterval <= Time.time)
                        {
                            weights.Clear();
                            this.FillAndPrepareWeights(category, weights, true);
                            this.gizmoLastUpdated = Time.time;
                        }

                        WeightedItem topWeightItem = null;
                        float topWeight = 0f;

                        foreach (var weightItem in weights)
                        {
                            this.DrawWeightedItemGizmos(weightItem);

                            if (weightItem.Weight > topWeight)
                            {
                                topWeightItem = weightItem;
                                topWeight = weightItem.Weight;
                            }
                        }

                        if (this.drawTopWeightGizmo && topWeightItem != null)
                        {
                            var s = new Vector3(this.gizmoMaxSize, this.gizmoMaxHeight, this.gizmoMaxSize);
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireCube(topWeightItem.Target.TargetPosition + Vector3.up * s.y / 2f, s);

                            topWeightItem = null;
                            topWeight = 0f;
                        }
                    }
                }
            }
        }

        private void DrawWeightedItemGizmos(WeightedItem weightItem)
        {
            if (weightItem.Target != null)
            {
                var w = Mathf.Min(weightItem.Weight, this.gizmoMaxWeight);
                var p = weightItem.Target.TargetPosition;
                var s = this.gizmoMaxSize / this.gizmoMaxWeight * w;
                var h = this.gizmoMaxHeight / this.gizmoMaxWeight * w;
                var c = Color.Lerp(Color.red, Color.green, 1f / this.gizmoMaxWeight * w);
                c.a = 0.75f;
                
                Gizmos.color = c;
                Gizmos.DrawCube(p + Vector3.up * h / 2f, new Vector3(s, h, s));            
            }            
        }

        public void FillWeights(string category, List<WeightedItem> weights)
        {
            for (int i=0; i < this.weightCalculators.Count; i++)
            {
				this.weightCalculators[i].FillWeights(category, weights);
            }
        }
        
        public void FillAndPrepareWeights(string category, List<WeightedItem> weights, bool excludeZeroWeight = false)
        {
            this.FillWeights(category, weights);

            weights.Sort();
            
            if (excludeZeroWeight)
                weights.RemoveAll(x => x.Weight == 0);
        }
    }
}