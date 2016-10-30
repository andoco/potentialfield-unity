namespace Andoco.Unity.Framework.Weighting
{
    using UnityEngine;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Andoco.Core;
    using Andoco.Unity.Framework.Core;

    public abstract class WeightCalculator : MonoBehaviour {

        private IList<IWeightCollector> collectors = new List<IWeightCollector>();
        private Dictionary<string, IList<WeightedItem>> weights = new Dictionary<string, IList<WeightedItem>>();
        private ITickedReceipt tickReceipt;

        public string weightName;
		public TickConfig tickConfig;

        #region Lifecycle

		void Start()
		{
			this.StartWeightUpdate();
		}

		void Spawned()
		{
			this.StartWeightUpdate();
		}

        void Recycled()
        {
			this.StopWeightUpdate();
        }

		void OnDestroy()
		{
			this.StopWeightUpdate();
		}

        #endregion

		public void FillWeights(string category, List<WeightedItem> weights)
        {
			IList<WeightedItem> matchingWeights;
			if (this.weights.TryGetValue(category, out matchingWeights))
			{
				weights.AddRange(matchingWeights);
			}
        }

        protected void AddCollector(IWeightCollector collector)
        {
            this.collectors.Add(collector);
            this.weights.Add(collector.Category, new List<WeightedItem>());
			Debug.Log(string.Format("Added weight collector {0}", collector.Category));
        }

		private void UpdateWeights()
		{
//			var bname = string.Format("UpdateWeights ({0})", this.weightName);
//			Benchy.Begin(bname);
			this.FillWeights();

            for (int i=0; i < this.collectors.Count; i++)
            {
                var collector = this.collectors[i];
                IList<WeightedItem> weights;
                if (this.weights.TryGetValue(collector.Category, out weights))
                {
                    weights.Clear();
                    collector.FillWeights(weights);
                }
            }

//			Benchy.End(bname);
		}

		#region Template methods

		protected virtual void OnStarted()
		{
		}

		protected virtual void OnStopped()
		{
		}

        protected virtual void FillWeights()
        {
        }

		#endregion

		private void StartWeightUpdate()
		{
			this.tickReceipt = this.ScheduleTick(this.tickConfig, this.UpdateWeights);
			this.OnStarted();
		}

		private void StopWeightUpdate()
		{
			this.tickReceipt.Cancel();
			this.OnStopped();
		}
    }
}