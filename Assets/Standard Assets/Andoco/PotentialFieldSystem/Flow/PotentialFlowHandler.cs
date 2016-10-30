using Andoco.Unity.Framework.Core;
using UnityEngine;
using UnityEngine.Assertions;

namespace Andoco.Unity.Framework.PotentialField
{
	public class PotentialFlowHandler
	{
		private readonly SimpleGraph graph;
		private readonly PotentialFieldSystem.Layer[] layers;
		private readonly PotentialData[][] layerData;

		public float Decay { get; set; }

		public float Momentum { get; set; }

		public PotentialFlowHandler(SimpleGraph graph, PotentialFieldSystem.Layer[] layers)
		{
			this.layers = layers;
			this.graph = graph;
			this.layerData = new PotentialData[layers.Length][];

			for (int i = 0; i < layers.Length; i++)
			{
                Assert.AreEqual(PotentialLayerKind.Flow, layers[i].Kind);

				this.layerData[i] = new PotentialData[graph.NumNodes];

				for (int j = 0; j < graph.NumNodes; j++)
				{
					this.layerData[i][j] = new PotentialData();
				}
			}
		}

		public void Propagate()
		{
			for (int i = 0; i < this.layers.Length; i++)
			{
				var layer = this.layers[i];
                Assert.AreEqual(PotentialLayerKind.Flow, layer.Kind);

				var potentialData = this.layerData[i];

				for (int j = 0; j < layer.Sources.Count; j++)
				{
					var source = layer.Sources[j];

                    Assert.AreNotEqual(PotentialFlowKind.None, source.Flow, string.Format("The potential source {0} with context {1} on layer {2} has no flow mode configured", source.SourceKey, source.Context, layer.LayerNum));

                    if (!source.Enabled)
                        continue;

					var graphNode = (SimpleGraphFieldNodeRef)source.Node;
					var data = potentialData[graphNode.Node];

					PotentialBlendMode? blendMode = null;

					switch (source.Flow)
					{
						case PotentialFlowKind.Block:
							blendMode = PotentialBlendMode.Block;
							break;
						case PotentialFlowKind.Propagate:
							blendMode = PotentialBlendMode.Normal;
							break;
						default:
							throw new System.InvalidOperationException(string.Format("Unknown potential flow kind {0}", source.Flow));
					}

					if (blendMode.HasValue)
						data.SetPotential(source.Potential, blendMode.Value);
				}

				UpdatePropagation(potentialData);
				UpdatePotentialBuffer(potentialData);
			}
		}

		public float Sample(int layerNum, int nodeIdx)
		{
			for (int i = 0; i < this.layers.Length; i++)
			{
				if (this.layers[i].LayerNum == layerNum)
				{
					return this.layerData[i][nodeIdx].Potential;
				}
			}

			throw new System.ArgumentOutOfRangeException("layerNum", layerNum, "The potential flow data cannot be found for the supplied layer number");
		}

		private void UpdatePropagation(PotentialData[] potentialData)
		{
			for (int i = 0; i < this.graph.NumNodes; i++)
			{
				var data = potentialData[i];

				// Blocking nodes never receive potential from neighbours.
				if (data.IsBlocking && !data.IsPropagating)
					continue;

				float maxPotential = 0.0f;
				float minPotential = 0.0f;

				var numArcs = this.graph.NumArcsForNode(i);

				for (int j = 0; j < numArcs; j++)
				{
					var neighbourData = potentialData[this.graph.GetNodeArc(i, j)];
					float potential = neighbourData.BufferedPotential * Mathf.Exp(-this.Decay); // TODO: Support an arc cost?

					if (neighbourData.IsBlocking && !neighbourData.IsPropagating)
						potential = 0f;

					maxPotential = Mathf.Max(potential, maxPotential);
					minPotential = Mathf.Min(potential, minPotential);
				}

				if (data.IsPropagating)
				{
					// We keep the propagating node potential if it has greater magnitude than any neighbour.
					maxPotential = Mathf.Max(data.BufferedPotential, maxPotential);
					minPotential = Mathf.Min(data.BufferedPotential, minPotential);
				}

				if (Mathf.Abs(minPotential) > maxPotential)
				{
					// A neighbour has greater negative potential.
					data.Potential = Mathf.Lerp(data.BufferedPotential, minPotential, this.Momentum);
				}
				else
				{
					// A neighbour has greater positive potential.
					data.Potential = Mathf.Lerp(data.BufferedPotential, maxPotential, this.Momentum);
				}
			}
		}

		private void UpdatePotentialBuffer(PotentialData[] potentialData)
		{
			for (int i = 0; i < potentialData.Length; i++)
			{
				var data = potentialData[i];
				data.BufferedPotential = data.Potential;
				data.IsPropagating = false;
			}
		}
	}
}
