using System;
using UnityEngine;

namespace Andoco.Unity.Framework.PotentialField
{
	public class PotentialData
	{
		public IFieldNodeRef Node;
		public float Potential;
		public float BufferedPotential;
		public bool IsPropagating;
		public bool IsBlocking;

		/// <summary>
		/// Configures the potential for the node.
		/// </summary>
        /// <remarks>
        /// Nodes can propagate outgoing potential, and block incoming potential at the same time.
        /// </remarks>
		public void SetPotential(float potential, PotentialBlendMode mode)
		{
			switch (mode)
			{
				case PotentialBlendMode.Normal:
					if (this.IsPropagating)
					{
						// Potential has already been set, so we keep whichever has the greater magnitude.
						if (Mathf.Abs(potential) > Mathf.Abs(this.Potential))
						{
							this.Potential = potential;
						}
					}
					else if (potential != 0f)
					{
						// Nothing else has set the potential yet, so we propagate as normal.
						this.Potential = potential;
						this.IsPropagating = true;
					}
					break;
				case PotentialBlendMode.Block:
					this.IsBlocking = true;
					break;
				default:
					throw new InvalidOperationException(string.Format("Unknown potential blend mode {0}", mode));
			}

			this.BufferedPotential = this.Potential;
		}
	}
}
