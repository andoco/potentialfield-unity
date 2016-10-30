namespace Andoco.Unity.Framework.Core.Meshes
{
    using UnityEngine;

    public struct Edge
    {
        public Vector3 v1;
        public Vector3 v2;

        public Edge(Vector3 v1, Vector3 v2)
        {
            if (v1.x < v2.x || (v1.x == v2.x && (v1.y < v2.y || (v1.y == v2.y && v1.z <= v2.z))))
            {
                this.v1 = v1;
                this.v2 = v2;
            }
            else
            {
                this.v1 = v2;
                this.v2 = v1;
            }
        }

        /// <summary>
        /// Checks if <paramref name="v"/> is one of the two vertices of this edge.
        /// </summary>
        /// <returns><c>true</c> if the vertex is part of the edge, otherwise <c>false</c>.</returns>
        /// <param name="v">The vertex to check.</param>
        public bool IsEndpoint(Vector3 v)
        {
            return this.v1 == v || this.v2 == v;
        }

        /// <summary>
        /// Gets the vertex at the other end of the edge from <paramref name="v"/>.
        /// </summary>
        /// <returns>The opposite vertex.</returns>
        /// <param name="v">The vertex to get the opposite vertex for.</param>
        public Vector3 GetOppositeVertex(Vector3 v)
        {
            if (v == this.v1)
                return this.v2;
            else if (v == this.v2)
                return this.v1;

            throw new System.InvalidOperationException(string.Format("Cannot get the opposite vertex because the supplied vertex {0} is not part of this edge", v));
        }
    }
}